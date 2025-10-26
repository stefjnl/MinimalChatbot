namespace MinimalChatbot.Application.DTOs;

public record ProviderHealthResponse(
    string ProviderName,
    bool IsHealthy);
