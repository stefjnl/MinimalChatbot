using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MinimalChatbot.Infrastructure.Data;

namespace MinimalChatbot.Tests.Support;

/// <summary>
/// Utilities for creating SQLite in-memory DbContext instances for tests.
/// </summary>
internal static class SqliteDbContextFactory
{
    public static async Task<SqliteConnection> CreateOpenConnectionAsync()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        await connection.OpenAsync().ConfigureAwait(false);
        return connection;
    }

    public static DbContextOptions<MinimalChatbotDbContext> CreateOptions(SqliteConnection connection)
    {
        return new DbContextOptionsBuilder<MinimalChatbotDbContext>()
            .UseSqlite(connection)
            .Options;
    }

    public static async Task EnsureCreatedAsync(DbContextOptions<MinimalChatbotDbContext> options)
    {
        await using var context = new MinimalChatbotDbContext(options);
        await context.Database.EnsureCreatedAsync().ConfigureAwait(false);
    }
}
