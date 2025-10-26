using System.Collections.Generic;

namespace MinimalChatbot.Domain.Models;

/// <summary>
/// Represents a message in a conversation.
/// </summary>
public class ConversationMessage
{
    /// <summary>
    /// Gets the ID of the message.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the role of the message sender.
    /// </summary>
    public MessageRole Role { get; init; }

    /// <summary>
    /// Gets the content of the message.
    /// </summary>
    public string Content { get; init; } = string.Empty;

    /// <summary>
    /// Gets the timestamp when the message was created.
    /// </summary>
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets the tool calls associated with this message.
    /// </summary>
    public IList<MessageToolCall> ToolCalls { get; init; } = new List<MessageToolCall>();

    /// <summary>
    /// Gets the artifacts associated with this message.
    /// </summary>
    public IList<MessageArtifact> Artifacts { get; init; } = new List<MessageArtifact>();

    /// <summary>
    /// Gets additional metadata for the message.
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = new();

    /// <summary>
    /// Creates a user message.
    /// </summary>
    /// <param name="content">The message content.</param>
    /// <returns>A user message.</returns>
    public static ConversationMessage User(string content) => new()
    {
        Role = MessageRole.User,
        Content = content
    };

    /// <summary>
    /// Creates an assistant message.
    /// </summary>
    /// <param name="content">The message content.</param>
    /// <returns>An assistant message.</returns>
    public static ConversationMessage Assistant(string content) => new()
    {
        Role = MessageRole.Assistant,
        Content = content
    };

    /// <summary>
    /// Creates a system message.
    /// </summary>
    /// <param name="content">The message content.</param>
    /// <returns>A system message.</returns>
    public static ConversationMessage System(string content) => new()
    {
        Role = MessageRole.System,
        Content = content
    };
}

/// <summary>
/// Represents the role of a message sender.
/// </summary>
public enum MessageRole
{
    /// <summary>
    /// The message is from the user.
    /// </summary>
    User,

    /// <summary>
    /// The message is from the AI assistant.
    /// </summary>
    Assistant,

    /// <summary>
    /// The message is from the system.
    /// </summary>
    System,

    /// <summary>
    /// The message is from a tool.
    /// </summary>
    Tool
}

/// <summary>
/// Represents a tool call in a message.
/// </summary>
public class MessageToolCall
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
}

/// <summary>
/// Represents an artifact in a message.
/// </summary>
public class MessageArtifact
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
