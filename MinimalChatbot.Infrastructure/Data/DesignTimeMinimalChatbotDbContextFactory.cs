using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MinimalChatbot.Infrastructure.Data;

/// <summary>
/// Provides design-time construction for <see cref="MinimalChatbotDbContext"/> to support EF Core tooling.
/// </summary>
public sealed class DesignTimeMinimalChatbotDbContextFactory : IDesignTimeDbContextFactory<MinimalChatbotDbContext>
{
    public MinimalChatbotDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MinimalChatbotDbContext>();
        optionsBuilder.UseSqlite("Data Source=minimalchatbot.db");
        return new MinimalChatbotDbContext(optionsBuilder.Options);
    }
}
