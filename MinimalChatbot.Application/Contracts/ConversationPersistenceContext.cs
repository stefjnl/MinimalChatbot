using System;
using MinimalChatbot.Domain.Entities;

namespace MinimalChatbot.Application.Contracts;

/// <summary>
/// Context data required to persist messages within a conversation.
/// </summary>
public sealed record ConversationPersistenceContext(
    Guid? ConversationId,
    string? UserId,
    ConversationProvider Provider,
    string ModelName,
    string? TitleSource,
    string? ThreadId);
