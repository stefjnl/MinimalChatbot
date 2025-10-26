namespace MinimalChatbot.Domain.Models;

/// <summary>
/// Represents an event in a streaming chat response.
/// </summary>
public class ChatStreamEvent
{
    /// <summary>
    /// Gets the type of the stream event.
    /// </summary>
    public ChatStreamEventType EventType { get; init; }

    /// <summary>
    /// Gets the conversation ID.
    /// </summary>
    public string ConversationId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the message ID.
    /// </summary>
    public string MessageId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the content chunk for message events.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets the tool call information for tool events.
    /// </summary>
    public ChatStreamToolCall? ToolCall { get; init; }

    /// <summary>
    /// Gets the artifact information for artifact events.
    /// </summary>
    public ChatStreamArtifact? Artifact { get; init; }

    /// <summary>
    /// Gets the error information for error events.
    /// </summary>
    public string? Error { get; init; }

    /// <summary>
    /// Gets the timestamp when the event was generated.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets whether this is the final event in the stream.
    /// </summary>
    public bool IsFinal { get; init; }

    /// <summary>
    /// Creates a message content event.
    /// </summary>
    /// <param name="conversationId">The conversation ID.</param>
    /// <param name="messageId">The message ID.</param>
    /// <param name="content">The content chunk.</param>
    /// <param name="isFinal">Whether this is the final chunk.</param>
    /// <returns>A message content stream event.</returns>
    public static ChatStreamEvent Message(string conversationId, string messageId, string content, bool isFinal = false) => new()
    {
        EventType = ChatStreamEventType.Message,
        ConversationId = conversationId,
        MessageId = messageId,
        Content = content,
        IsFinal = isFinal
    };

    /// <summary>
    /// Creates a tool call event.
    /// </summary>
    /// <param name="conversationId">The conversation ID.</param>
    /// <param name="messageId">The message ID.</param>
    /// <param name="toolCall">The tool call information.</param>
    /// <returns>A tool call stream event.</returns>
    public static ChatStreamEvent ToolCallEvent(string conversationId, string messageId, ChatStreamToolCall toolCall) => new()
    {
        EventType = ChatStreamEventType.ToolCall,
        ConversationId = conversationId,
        MessageId = messageId,
        ToolCall = toolCall
    };

    /// <summary>
    /// Creates an artifact event.
    /// </summary>
    /// <param name="conversationId">The conversation ID.</param>
    /// <param name="messageId">The message ID.</param>
    /// <param name="artifact">The artifact information.</param>
    /// <returns>An artifact stream event.</returns>
    public static ChatStreamEvent ArtifactEvent(string conversationId, string messageId, ChatStreamArtifact artifact) => new()
    {
        EventType = ChatStreamEventType.Artifact,
        ConversationId = conversationId,
        MessageId = messageId,
        Artifact = artifact
    };

    /// <summary>
    /// Creates an error event.
    /// </summary>
    /// <param name="conversationId">The conversation ID.</param>
    /// <param name="messageId">The message ID.</param>
    /// <param name="error">The error message.</param>
    /// <returns>An error stream event.</returns>
    public static ChatStreamEvent ErrorEvent(string conversationId, string messageId, string error) => new()
    {
        EventType = ChatStreamEventType.Error,
        ConversationId = conversationId,
        MessageId = messageId,
        Error = error,
        IsFinal = true
    };
}

/// <summary>
/// Represents the type of a chat stream event.
/// </summary>
public enum ChatStreamEventType
{
    /// <summary>
    /// A message content chunk.
    /// </summary>
    Message,

    /// <summary>
    /// A tool call invocation.
    /// </summary>
    ToolCall,

    /// <summary>
    /// An artifact generation.
    /// </summary>
    Artifact,

    /// <summary>
    /// An error occurred.
    /// </summary>
    Error,

    /// <summary>
    /// Stream started.
    /// </summary>
    Started,

    /// <summary>
    /// Stream completed.
    /// </summary>
    Completed
}

/// <summary>
/// Represents a tool call in a stream event.
/// </summary>
public class ChatStreamToolCall
{
    /// <summary>
    /// Gets the ID of the tool call.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the name of the tool.
    /// </summary>
    public string ToolName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the arguments for the tool call.
    /// </summary>
    public string Arguments { get; init; } = string.Empty;

    /// <summary>
    /// Gets the status of the tool call.
    /// </summary>
    public ToolCallStreamStatus Status { get; init; }
}

/// <summary>
/// Represents the status of a tool call in a stream.
/// </summary>
public enum ToolCallStreamStatus
{
    /// <summary>
    /// Tool call is starting.
    /// </summary>
    Started,

    /// <summary>
    /// Tool call is in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// Tool call completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// Tool call failed.
    /// </summary>
    Failed
}

/// <summary>
/// Represents an artifact in a stream event.
/// </summary>
public class ChatStreamArtifact
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
}
