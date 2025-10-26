namespace MinimalChatbot.Application;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalChatbot.Application.Contracts;
using MinimalChatbot.Application.Services;
using MinimalChatbot.Domain.Ports;

public static class DependencyInjection
{
    public static IServiceCollection AddAIProviderApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // ========================================================================
        // Configuration (Application Layer - Orchestration Settings)
        // ========================================================================
        
        services.Configure<AIProviderOptions>(configuration.GetSection(AIProviderOptions.SectionName));

        // ========================================================================
        // Application Services
        // ========================================================================
        
        services.AddScoped<IProviderSelectionStrategy, HealthBasedProviderSelection>();
        services.AddScoped<AIProvider>();

        return services;
    }
}
