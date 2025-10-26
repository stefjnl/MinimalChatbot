namespace MinimalChatbot.Application.DI
{
    using Microsoft.Extensions.DependencyInjection;
    using MinimalChatbot.Application.UseCases;

    /// <summary>
    /// Extension methods for registering Application layer services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds MinimalChatbot Application services to the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns>The service collection with added services.</returns>
        public static IServiceCollection AddMinimalChatbotApplication(this IServiceCollection services)
        {
            services.AddScoped<StreamChatUseCase>();
            return services;
        }
    }
}
