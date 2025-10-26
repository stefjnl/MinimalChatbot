using System.Collections.Generic;

namespace MinimalChatbot.Domain.Models;

/// <summary>
/// Represents a request to an AI provider.
/// </summary>
public class AIRequest
{
    /// <summary>
    /// Gets the messages in the conversation.
    /// </summary>
    public IList<ConversationMessage> Messages { get; init; } = new List<ConversationMessage>();

    /// <summary>
    /// Gets the maximum number of tokens to generate.
    /// </summary>
    public int? MaxTokens { get; init; }

    /// <summary>
    /// Gets the temperature for controlling randomness in responses.
    /// </summary>
    public double? Temperature { get; init; }

    /// <summary>
    /// Gets the system prompt or instructions.
    /// </summary>
    public string? SystemPrompt { get; init; }

    /// <summary>
    /// Gets the tools available for the AI to use.
    /// </summary>
    public IList<AITool> Tools { get; init; } = new List<AITool>();

    /// <summary>
    /// Gets additional metadata for the request.
    /// </summary>
    public Dictionary<string, object> Metadata { get; init; } = new();
}

/// <summary>
/// Represents a tool that can be used by the AI.
/// </summary>
public class AITool
{
    /// <summary>
    /// Gets the name of the tool.
    /// </summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Gets the description of what the tool does.
    /// </summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Gets the parameters schema for the tool.
    /// </summary>
    public object Parameters { get; init; } = new();
}
