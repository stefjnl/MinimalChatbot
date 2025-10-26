using System.Collections.Generic;

namespace MinimalChatbot.Domain.Models;

/// <summary>
/// Represents a response from an AI provider.
/// </summary>
public class AIResponse
{
    /// <summary>
    /// Gets the content of the response.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets the tool calls made by the AI.
    /// </summary>
    public IList<AIToolCall> ToolCalls { get; init; } = new List<AIToolCall>();

    /// <summary>
    /// Gets the usage metadata for the response.
    /// </summary>
    public AIUsage? Usage { get; init; }

    /// <summary>
    /// Gets the finish reason for the response.
    /// </summary>
    public string? FinishReason { get; init; }

    /// <summary>
    /// Gets additional metadata from the AI provider.
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = new();
}

/// <summary>
/// Represents a tool call made by the AI.
/// </summary>
public class AIToolCall
{
    /// <summary>
    /// Gets the ID of the tool call.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the tool that was called.
    /// </summary>
    public string ToolName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the arguments passed to the tool.
    /// </summary>
    public Dictionary<string, object?> Arguments { get; init; } = new();
}

/// <summary>
/// Represents usage metadata for an AI response.
/// </summary>
public class AIUsage
{
    /// <summary>
    /// Gets the number of prompt tokens used.
    /// </summary>
    public int PromptTokens { get; init; }

    /// <summary>
    /// Gets the number of completion tokens used.
    /// </summary>
    public int CompletionTokens { get; init; }

    /// <summary>
    /// Gets the total number of tokens used.
    /// </summary>
    public int TotalTokens => PromptTokens + CompletionTokens;
}
