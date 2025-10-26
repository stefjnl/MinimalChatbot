using System.Collections.Generic;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MinimalChatbot.Domain.Models;
using System.Text.Json;

namespace MinimalChatbot.Web.Streaming
{
    /// <summary>
    /// Provides centralized logic for writing Server-Sent Events (SSE) for chat streaming.
    /// </summary>
    public static class StreamingEventWriter
    {
        private static readonly Meter Meter = new("MinimalChatbot.Web.Streaming", "1.0.0");
        private static readonly Counter<long> ToolInvocationCounter = Meter.CreateCounter<long>("mcp.tool.invoked");
        private static readonly JsonSerializerOptions ToolPayloadSerializerOptions = new(JsonSerializerDefaults.Web)
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        /// <summary>
        /// Writes chat stream events as Server-Sent Events to the HTTP response.
        /// </summary>
        /// <param name="context">The HTTP context for the response.</param>
        /// <param name="events">The asynchronous enumerable of chat stream events.</param>
        /// <param name="logger">The logger for recording events and errors.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task WriteAsync(HttpContext context, IAsyncEnumerable<ChatStreamEvent> events, ILogger logger, CancellationToken ct = default)
        {
            context.Response.ContentType = "text/event-stream";
            context.Response.Headers.CacheControl = "no-cache";
            context.Response.Headers.Connection = "keep-alive";

            var updateCount = 0;

            try
            {
                await foreach (var update in events)
                {
                    updateCount++;

                    if (update.EventType == ChatStreamEventType.Started)
                    {
                        logger.LogInformation("Sending stream started event");
                        await context.Response.WriteAsync($"event: started\ndata: {{ \"conversationId\": \"{update.ConversationId}\" }}\n\n", ct);
                        await context.Response.Body.FlushAsync(ct);
                    }
                    else if (update.EventType == ChatStreamEventType.ToolCall && update.ToolCall != null)
                    {
                        var toolName = update.ToolCall.ToolName ?? "unknown";
                        if (update.ToolCall.Status == ToolCallStreamStatus.Started)
                        {
                            ToolInvocationCounter.Add(1, KeyValuePair.Create<string, object?>("tool", toolName));
                        }

                        var payload = !string.IsNullOrWhiteSpace(update.ToolCall.Arguments)
                            ? update.ToolCall.Arguments
                            : JsonSerializer.Serialize(new {
                                id = update.ToolCall.Id,
                                name = toolName,
                                status = update.ToolCall.Status.ToString()
                            }, ToolPayloadSerializerOptions);

                        logger.LogInformation("Sending tool_call event: {ToolName} (status: {Status})", toolName, update.ToolCall.Status);
                        await context.Response.WriteAsync($"event: tool_call\ndata: {payload}\n\n", ct);
                        await context.Response.Body.FlushAsync(ct);
                    }
                    else if (update.EventType == ChatStreamEventType.Artifact && update.Artifact != null)
                    {
                        logger.LogInformation("Sending artifact event: {Type} - {Title}", update.Artifact.Type, update.Artifact.Title);
                        var payload = JsonSerializer.Serialize(new {
                            id = update.Artifact.Id,
                            type = update.Artifact.Type,
                            title = update.Artifact.Title,
                            content = update.Artifact.Content
                        }, ToolPayloadSerializerOptions);
                        await context.Response.WriteAsync($"event: artifact\ndata: {payload}\n\n", ct);
                        await context.Response.Body.FlushAsync(ct);
                    }
                    else if (update.EventType == ChatStreamEventType.Error && !string.IsNullOrEmpty(update.Error))
                    {
                        logger.LogInformation("Sending error event: {Error}", update.Error);
                        await context.Response.WriteAsync($"event: error\ndata: {update.Error}\n\n", ct);
                        await context.Response.Body.FlushAsync(ct);
                    }
                    else if (update.EventType == ChatStreamEventType.Message && !string.IsNullOrEmpty(update.Content))
                    {
                        var escapedContent = update.Content.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t");
                        logger.LogInformation("Sending token #{Count}: RAW='{Escaped}' (Length={Length})",
                            updateCount,
                            escapedContent.Length > 100 ? escapedContent.Substring(0, 100) + "..." : escapedContent,
                            update.Content.Length);

                        await context.Response.WriteAsync($"data: {update.Content}\n\n", ct);
                        await context.Response.Body.FlushAsync(ct);
                    }
                    else if (update.EventType == ChatStreamEventType.Completed)
                    {
                        logger.LogInformation("Sending stream completed event");
                        await context.Response.WriteAsync($"event: completed\ndata: {{ \"conversationId\": \"{update.ConversationId}\", \"messageId\": \"{update.MessageId}\" }}\n\n", ct);
                        await context.Response.Body.FlushAsync(ct);
                    }
                }

                logger.LogInformation("Stream complete. Sending [DONE] marker after {Count} updates", updateCount);
                await context.Response.WriteAsync("data: [DONE]\n\n", ct);
                await context.Response.Body.FlushAsync(ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during streaming after {Count} updates", updateCount);
                await context.Response.WriteAsync($"event: error\ndata: {ex.Message}\n\n", ct);
                await context.Response.Body.FlushAsync(ct);
            }
        }

    }
}
