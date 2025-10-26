using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MinimalChatbot.Application.Services;
using MinimalChatbot.Domain.Ports;
using MinimalChatbot.Infrastructure.Configuration;
using MinimalChatbot.Infrastructure.LLM;

namespace MinimalChatbot.Infrastructure.DI;

/// <summary>
/// Extension methods for configuring MinimalChatbot services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds MinimalChatbot services to the dependency injection container.
    /// </summary>
    /// <param name="services">The service collection to add services to.</param>
    /// <param name="configuration">The application configuration.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddMinimalChatbot(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration);

        // Register agent factory
        services.AddSingleton<IAIAgentFactory>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<AppSettings>>();
            var configuration = sp.GetRequiredService<IConfiguration>();
            var logger = sp.GetRequiredService<ILogger<ConfiguredAIAgentFactory>>();
            return new ConfiguredAIAgentFactory(options, configuration, logger);
        });

        return services;
    }
}
