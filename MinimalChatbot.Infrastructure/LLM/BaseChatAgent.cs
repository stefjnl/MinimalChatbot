using Microsoft.Extensions.AI;
using Microsoft.Agents.AI;
using MinimalChatbot.Domain.Models;
using MinimalChatbot.Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DomainChatResponse = MinimalChatbot.Domain.Models.ChatResponse;

namespace MinimalChatbot.Infrastructure.LLM;

/// <summary>
/// Abstract base class for ChatAgent implementations that provides shared logic
/// for streaming, tool handling, usage extraction, and IAIClient implementation.
/// Providers should inherit this and implement IChatAgent explicitly for provider-specific behavior.
/// </summary>
public abstract class BaseChatAgent
{
    protected readonly IChatClient _chatClient;
    protected readonly ChatClientAgent _agent;
    protected readonly IAgentThreadProvider? _threadProvider;
    protected readonly string _configuredModel;

    protected BaseChatAgent(
        IChatClient chatClient,
        string agentName,
        string instructions,
        IEnumerable<object>? tools,
        IAgentThreadProvider? threadProvider,
        string configuredModel)
    {
        _chatClient = chatClient ?? throw new ArgumentNullException(nameof(chatClient));
        _configuredModel = configuredModel ?? throw new ArgumentNullException(nameof(configuredModel));
        _threadProvider = threadProvider;

        var aiTools = tools?.OfType<Domain.Models.AITool>().ToArray() ?? Array.Empty<Domain.Models.AITool>();
        _agent = new ChatClientAgent(
            _chatClient,
            name: agentName,
            instructions: instructions ?? string.Empty,
            tools: null);
    }

    /// <summary>
    /// Shared implementation for SendAsync - providers can delegate to this.
    /// </summary>
    protected async Task<DomainChatResponse> SendAsyncCore(
        string message,
        string? threadId = null,
        CancellationToken ct = default)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        AgentThread? thread = null;
        if (!string.IsNullOrEmpty(threadId) && _threadProvider is not null)
        {
            thread = await _threadProvider.GetThreadAsync(threadId, ct).ConfigureAwait(false);
        }

        var response = thread is not null
            ? await _agent.RunAsync(message, thread, cancellationToken: ct).ConfigureAwait(false)
            : await _agent.RunAsync(message, cancellationToken: ct).ConfigureAwait(false);

        var text = response?.Text ?? string.Empty;
        var usage = response?.Usage;
        return new DomainChatResponse
        {
            Message = text,
            Provider = _configuredModel,
            Usage = usage != null ? new ChatUsage
            {
                PromptTokens = ExtractPromptTokens(usage),
                CompletionTokens = ExtractCompletionTokens(usage)
            } : null
        };
    }

    /// <summary>
    /// Shared implementation for StreamAsync - providers can delegate to this.
    /// </summary>
    protected async IAsyncEnumerable<ChatStreamEvent> StreamAsyncCore(
        string message,
        string? threadId = null,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        if (message is null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        AgentThread? thread = null;
        if (!string.IsNullOrEmpty(threadId) && _threadProvider is not null)
        {
            thread = await _threadProvider.GetThreadAsync(threadId, ct).ConfigureAwait(false);
        }

        var stream = thread is not null
            ? _agent.RunStreamingAsync(message, thread, cancellationToken: ct)
            : _agent.RunStreamingAsync(message, cancellationToken: ct);

        await foreach (var update in stream.WithCancellation(ct).ConfigureAwait(false))
        {
            if (!string.IsNullOrEmpty(update.Text))
            {
                yield return ChatStreamEvent.Message(
                    conversationId: string.Empty,
                    messageId: Guid.NewGuid().ToString(),
                    content: update.Text);
            }

            if (update.Contents is not null)
            {
                foreach (var content in update.Contents)
                {
                    if (content is FunctionCallContent fcc && !string.IsNullOrEmpty(fcc.Name))
                    {
                        var payload = ToolStreamingFormatter.CreatePayload(
                            fcc.Name, "call", arguments: fcc.Arguments);
                        yield return ChatStreamEvent.ToolCallEvent(
                            conversationId: string.Empty,
                            messageId: Guid.NewGuid().ToString(),
                            toolCall: new ChatStreamToolCall
                            {
                                Id = Guid.NewGuid().ToString(),
                                ToolName = fcc.Name,
                                Arguments = payload,
                                Status = ToolCallStreamStatus.Started
                            });
                    }
                    else if (content is FunctionResultContent frc)
                    {
                        var toolName = ResolveToolName(frc);
                        var payload = ToolStreamingFormatter.CreatePayload(
                            toolName, "result", result: frc.Result);
                        yield return ChatStreamEvent.ToolCallEvent(
                            conversationId: string.Empty,
                            messageId: Guid.NewGuid().ToString(),
                            toolCall: new ChatStreamToolCall
                            {
                                Id = Guid.NewGuid().ToString(),
                                ToolName = toolName,
                                Arguments = payload,
                                Status = ToolCallStreamStatus.Completed
                            });

                        var actionCardJson = ExtractActionCardFromToolResult(frc.Result);
                        if (actionCardJson is not null)
                        {
                            yield return ChatStreamEvent.ArtifactEvent(
                                conversationId: string.Empty,
                                messageId: Guid.NewGuid().ToString(),
                                artifact: new ChatStreamArtifact
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Type = "actioncard",
                                    Title = "Action Card",
                                    Content = actionCardJson
                                });
                        }
                    }
                    else if (content is UsageContent usageContent)
                    {
                        var usagePayload = UsageMetadataBuilder.TryCreateUsagePayload(usageContent.Details);
                        if (!string.IsNullOrEmpty(usagePayload))
                        {
                            yield return ChatStreamEvent.ArtifactEvent(
                                conversationId: string.Empty,
                                messageId: Guid.NewGuid().ToString(),
                                artifact: new ChatStreamArtifact
                                {
                                    Id = Guid.NewGuid().ToString(),
                                    Type = "metadata",
                                    Title = "Usage Metadata",
                                    Content = usagePayload
                                });
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Shared implementation for CreateThreadAsync - providers can delegate to this.
    /// </summary>
    protected Task<AgentThread> CreateThreadAsyncCore(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(_agent.GetNewThread());
    }

    /// <summary>
    /// Shared implementation for CallAsync (IAIClient) - converts domain AIRequest to ChatMessage
    /// and invokes the IChatClient, then maps back to domain AIResponse.
    /// </summary>
    protected async Task<AIResponse> CallAsyncCore(
        AIRequest request,
        string providerName,
        CancellationToken ct = default)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var messages = new List<ChatMessage>
        {
            new(ChatRole.System, request.SystemPrompt ?? string.Empty),
            new(ChatRole.User, request.Messages.FirstOrDefault()?.Content ?? string.Empty)
        };

        var response = await _chatClient.GetResponseAsync(messages, cancellationToken: ct).ConfigureAwait(false);
        var text = response.Text ?? string.Empty;
        var tokens = ExtractTotalTokens(response.Usage);

        return new AIResponse
        {
            Content = text.Trim(),
            Metadata = new Dictionary<string, object>
            {
                ["Model"] = _configuredModel,
                ["TokensUsed"] = tokens,
                ["ProviderName"] = providerName
            }
        };
    }

    /// <summary>
    /// Extracts action card JSON if the tool result indicates PENDING_CONFIRMATION status.
    /// </summary>
    protected static string? ExtractActionCardFromToolResult(object? result)
    {
        if (result is null)
        {
            return null;
        }

        var resultType = result.GetType();
        var statusProp = resultType.GetProperty("Status");
        var actionCardJsonProp = resultType.GetProperty("ActionCardJson");

        if (statusProp is null || actionCardJsonProp is null)
        {
            return null;
        }

        var status = statusProp.GetValue(result)?.ToString();
        if (!string.Equals(status, "PENDING_CONFIRMATION", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return actionCardJsonProp.GetValue(result)?.ToString();
    }

    /// <summary>
    /// Resolves tool name from source object via reflection (fallback: "unknown").
    /// </summary>
    protected static string ResolveToolName(object source, string fallback = "unknown")
    {
        var name = source.GetType().GetProperty("Name")?.GetValue(source) as string;
        return string.IsNullOrWhiteSpace(name) ? fallback : name!;
    }

    /// <summary>
    /// Extracts total token count from usage metadata via reflection.
    /// Tolerates missing or null usage information.
    /// </summary>
    protected static int ExtractTotalTokens(UsageDetails? usage)
    {
        if (usage is null)
        {
            return 0;
        }

        try
        {
            var totalProp = usage.GetType().GetProperty("TotalTokenCount");
            if (totalProp is not null)
            {
                var value = totalProp.GetValue(usage);
                if (value is int intValue)
                {
                    return intValue;
                }
                if (value is long longValue)
                {
                    return (int)longValue;
                }
            }

            // Fallback: sum input + output
            var inputProp = usage.GetType().GetProperty("InputTokenCount");
            var outputProp = usage.GetType().GetProperty("OutputTokenCount");
            var inputTokens = inputProp?.GetValue(usage) as int? ?? 0;
            var outputTokens = outputProp?.GetValue(usage) as int? ?? 0;
            return inputTokens + outputTokens;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// Extracts prompt token count from usage metadata via reflection.
    /// </summary>
    protected static int ExtractPromptTokens(UsageDetails? usage)
    {
        if (usage is null)
        {
            return 0;
        }

        try
        {
            var inputProp = usage.GetType().GetProperty("InputTokenCount");
            if (inputProp is not null)
            {
                var value = inputProp.GetValue(usage);
                if (value is int intValue)
                {
                    return intValue;
                }
                if (value is long longValue)
                {
                    return (int)longValue;
                }
            }
        }
        catch
        {
            return 0;
        }

        return 0;
    }

    /// <summary>
    /// Extracts completion token count from usage metadata via reflection.
    /// </summary>
    protected static int ExtractCompletionTokens(UsageDetails? usage)
    {
        if (usage is null)
        {
            return 0;
        }

        try
        {
            var outputProp = usage.GetType().GetProperty("OutputTokenCount");
            if (outputProp is not null)
            {
                var value = outputProp.GetValue(usage);
                if (value is int intValue)
                {
                    return intValue;
                }
                if (value is long longValue)
                {
                    return (int)longValue;
                }
            }
        }
        catch
        {
            return 0;
        }

        return 0;
    }

    /// <summary>
    /// Default health check implementation using a lightweight HTTP GET or SDK metadata check.
    /// Providers can override if they need custom health check logic.
    /// </summary>
    protected virtual async Task<bool> IsHealthyCoreAsync(
        HttpClient httpClient,
        string healthEndpoint,
        CancellationToken ct = default)
    {
        try
        {
            var response = await httpClient.GetAsync(healthEndpoint, ct).ConfigureAwait(false);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Default GetModelsAsync implementation - returns configured model only.
    /// Providers can override to enumerate remote models if needed.
    /// </summary>
    protected virtual Task<IEnumerable<string>> GetModelsCoreAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult<IEnumerable<string>>(new[] { _configuredModel });
    }
}
