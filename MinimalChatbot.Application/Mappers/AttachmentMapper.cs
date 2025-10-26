using System;
using MinimalChatbot.Application.DTOs;
using MinimalChatbot.Domain.Entities;

namespace MinimalChatbot.Application.Mappers;

/// <summary>
/// Helper extensions to convert attachment entities to DTOs.
/// </summary>
public static class AttachmentMapper
{
    public static AttachmentDto ToDto(this Attachment attachment)
    {
        ArgumentNullException.ThrowIfNull(attachment);
        return new AttachmentDto(
            attachment.Id,
            attachment.ConversationId,
            attachment.Filename,
            attachment.Content,
            attachment.FileType,
            attachment.CreatedAt);
    }
}
