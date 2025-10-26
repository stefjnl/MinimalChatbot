using System.Collections.Generic;

namespace MinimalChatbot.Domain.Models;

/// <summary>
/// Represents a response from a chat request.
/// </summary>
public class ChatResponse
{
    /// <summary>
    /// Gets the conversation ID.
    /// </summary>
    public string ConversationId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the AI response message.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the message ID.
    /// </summary>
    public string MessageId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the provider that generated the response.
    /// </summary>
    public string Provider { get; init; } = string.Empty;

    /// <summary>
    /// Gets the artifacts generated during the chat.
    /// </summary>
    public IList<ChatArtifact> Artifacts { get; init; } = new List<ChatArtifact>();

    /// <summary>
    /// Gets the tool calls made by the AI.
    /// </summary>
    public IList<ChatToolCall> ToolCalls { get; init; } = new List<ChatToolCall>();

    /// <summary>
    /// Gets the usage information for the response.
    /// </summary>
    public ChatUsage? Usage { get; init; }

    /// <summary>
    /// Gets the timestamp when the response was generated.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

/// <summary>
/// Represents an artifact generated during a chat.
/// </summary>
public class ChatArtifact
{
    /// <summary>
    /// Gets the ID of the artifact.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the type of artifact.
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Gets the title of the artifact.
    /// </summary>
    public string Title { get; init; } = string.Empty;

    /// <summary>
    /// Gets the content of the artifact.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets the metadata associated with the artifact.
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = new();
}

/// <summary>
/// Represents a tool call made during a chat.
/// </summary>
public class ChatToolCall
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

    /// <summary>
    /// Gets the result of the tool call.
    /// </summary>
    public string? Result { get; init; }

    /// <summary>
    /// Gets the status of the tool call.
    /// </summary>
    public ToolCallStatus Status { get; init; }
}

/// <summary>
/// Represents the status of a tool call.
/// </summary>
public enum ToolCallStatus
{
    /// <summary>
    /// The tool call is pending execution.
    /// </summary>
    Pending,

    /// <summary>
    /// The tool call completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// The tool call failed.
    /// </summary>
    Failed
}

/// <summary>
/// Represents usage information for a chat response.
/// </summary>
public class ChatUsage
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

    /// <summary>
    /// Gets the cost of the request (if available).
    /// </summary>
    public decimal? Cost { get; init; }
}
