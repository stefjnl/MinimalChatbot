using System;
using System.Threading.Tasks;
using MinimalChatbot.Application.UseCases;
using MinimalChatbot.Infrastructure.Data;
using MinimalChatbot.Infrastructure.Data.Repositories;
using MinimalChatbot.Tests.Support;
using Xunit;

namespace MinimalChatbot.Tests.Application;

public sealed class LoadConversationUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_ReturnsConversationWithMessagesAndMetadata()
    {
        await using var connection = await SqliteDbContextFactory.CreateOpenConnectionAsync();
        var options = SqliteDbContextFactory.CreateOptions(connection);
        await SqliteDbContextFactory.EnsureCreatedAsync(options);

        var conversationId = Guid.NewGuid();
        await using (var arrangeContext = new MinimalChatbotDbContext(options))
        {
            var repository = new ConversationRepository(arrangeContext);
            var conversation = TestEntityFactory.CreateConversation(
                id: conversationId,
                includeMessages: false);

            // Add two messages with different payloads to verify ordering and nested data.
            conversation.Messages.Add(TestEntityFactory.CreateMessage(conversationId, includeActionCard: true, includeFileOperation: true));
            conversation.Messages.Add(TestEntityFactory.CreateMessage(conversationId, includeActionCard: false, includeFileOperation: false));

            await repository.CreateAsync(conversation);
        }

        await using (var assertContext = new MinimalChatbotDbContext(options))
        {
            var repository = new ConversationRepository(assertContext);
            var useCase = new LoadConversationUseCase(repository);

            var result = await useCase.ExecuteAsync(conversationId);

            Assert.NotNull(result);
            Assert.Equal(conversationId, result!.Id);
            Assert.Equal(2, result.Messages.Count);

            var firstMessage = result.Messages[0];
            Assert.NotNull(firstMessage.ActionCard);
            Assert.Single(firstMessage.PlannedActions);
            Assert.NotNull(firstMessage.FileOperation);

            var secondMessage = result.Messages[1];
            Assert.Null(secondMessage.ActionCard);
            Assert.Empty(secondMessage.PlannedActions);
            Assert.Null(secondMessage.FileOperation);
        }
    }
}
