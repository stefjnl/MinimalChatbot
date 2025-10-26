using System.Collections.Generic;

namespace MinimalChatbot.Domain.Models;

/// <summary>
/// Represents input for a chat request.
/// </summary>
public class ChatInput
{
    /// <summary>
    /// Gets the conversation ID.
    /// </summary>
    public string ConversationId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the user message.
    /// </summary>
    public string Message { get; init; } = string.Empty;

    /// <summary>
    /// Gets the attachments to include with the message.
    /// </summary>
    public IList<ChatAttachment> Attachments { get; init; } = new List<ChatAttachment>();

    /// <summary>
    /// Gets the selected AI provider.
    /// </summary>
    public string? Provider { get; init; }

    /// <summary>
    /// Gets additional options for the chat request.
    /// </summary>
    public Dictionary<string, object> Options { get; init; } = new();
}

/// <summary>
/// Represents an attachment in a chat message.
/// </summary>
public class ChatAttachment
{
    /// <summary>
    /// Gets the ID of the attachment.
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Gets the filename of the attachment.
    /// </summary>
    public string Filename { get; init; } = string.Empty;

    /// <summary>
    /// Gets the content type of the attachment.
    /// </summary>
    public string ContentType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the size of the attachment in bytes.
    /// </summary>
    public long Size { get; init; }

    /// <summary>
    /// Gets the URL or path to the attachment content.
    /// </summary>
    public string Url { get; init; } = string.Empty;
}
