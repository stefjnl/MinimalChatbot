using System;
using System.Threading.Tasks;
using MinimalChatbot.Application.UseCases;
using MinimalChatbot.Infrastructure.Data;
using MinimalChatbot.Infrastructure.Data.Repositories;
using MinimalChatbot.Tests.Support;
using Xunit;

namespace MinimalChatbot.Tests.Application;

public sealed class ArchiveConversationUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_MarksConversationAsArchived()
    {
        await using var connection = await SqliteDbContextFactory.CreateOpenConnectionAsync();
        var options = SqliteDbContextFactory.CreateOptions(connection);
        await SqliteDbContextFactory.EnsureCreatedAsync(options);

        var conversationId = Guid.NewGuid();
        await using (var arrangeContext = new MinimalChatbotDbContext(options))
        {
            var repository = new ConversationRepository(arrangeContext);
            await repository.CreateAsync(TestEntityFactory.CreateConversation(
                id: conversationId,
                includeMessages: true,
                isArchived: false));
        }

        await using (var assertContext = new MinimalChatbotDbContext(options))
        {
            var repository = new ConversationRepository(assertContext);
            var useCase = new ArchiveConversationUseCase(repository);

            var result = await useCase.ExecuteAsync(conversationId);

            Assert.NotNull(result);
            Assert.True(result!.IsArchived);

            var persisted = await repository.GetByIdAsync(conversationId, includeMessages: false);
            Assert.NotNull(persisted);
            Assert.True(persisted!.IsArchived);
        }
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsNull_WhenConversationDoesNotExist()
    {
        await using var connection = await SqliteDbContextFactory.CreateOpenConnectionAsync();
        var options = SqliteDbContextFactory.CreateOptions(connection);
        await SqliteDbContextFactory.EnsureCreatedAsync(options);

        await using var context = new MinimalChatbotDbContext(options);
        var useCase = new ArchiveConversationUseCase(new ConversationRepository(context));

        var result = await useCase.ExecuteAsync(Guid.NewGuid());

        Assert.Null(result);
    }
}
