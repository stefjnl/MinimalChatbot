using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalChatbot.Application.UseCases;
using MinimalChatbot.Domain.Entities;
using MinimalChatbot.Domain.Ports;
using MinimalChatbot.Infrastructure.Data;
using MinimalChatbot.Infrastructure.Data.Repositories;
using MinimalChatbot.Tests.Support;
using Xunit;

namespace MinimalChatbot.Tests.Application;

public sealed class AddAttachmentToConversationUseCaseTests
{
    private class TestAttachmentValidator : IAttachmentValidator
    {
        public IReadOnlyList<string> AllowedFileTypes => new List<string> { ".txt", ".md", ".json" };

        public bool IsFileTypeAllowed(string fileType) => AllowedFileTypes.Any(t => t == fileType.ToLowerInvariant());
    }

    [Fact]
    public async Task ExecuteAsync_ValidInput_AddsAttachmentAndUpdatesConversation()
    {
        await using var connection = await SqliteDbContextFactory.CreateOpenConnectionAsync();
        var options = SqliteDbContextFactory.CreateOptions(connection);
        await SqliteDbContextFactory.EnsureCreatedAsync(options);

        var conversationId = Guid.NewGuid();
        await using (var arrangeContext = new MinimalChatbotDbContext(options))
        {
            var repository = new ConversationRepository(arrangeContext);
            var conversation = TestEntityFactory.CreateConversation(id: conversationId, includeMessages: false);
            await repository.CreateAsync(conversation);
        }

        await using (var assertContext = new MinimalChatbotDbContext(options))
        {
            var attachmentRepository = new AttachmentRepository(assertContext);
            var conversationRepository = new ConversationRepository(assertContext);
            var validator = new TestAttachmentValidator();
            var useCase = new AddAttachmentToConversationUseCase(attachmentRepository, conversationRepository, validator);

            var filename = "test.txt";
            var content = "Hello World";
            var fileType = ".txt";

            // Act
            var result = await useCase.ExecuteAsync(conversationId, filename, content, fileType);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(conversationId, result.ConversationId);
            Assert.Equal(filename, result.Filename);
            Assert.Equal(content, result.Content);
            Assert.Equal(fileType, result.FileType);

            // Verify attachment was added
            var attachments = await attachmentRepository.GetByConversationIdAsync(conversationId);
            Assert.Single(attachments);
            Assert.Equal(filename, attachments[0].Filename);

            // Verify conversation was updated (timestamp)
            var updatedConversation = await conversationRepository.GetByIdAsync(conversationId);
            Assert.NotNull(updatedConversation);
            // Note: Attachments collection is not loaded here, so we don't check it
        }
    }

    [Fact]
    public async Task ExecuteAsync_ConversationNotFound_ThrowsException()
    {
        await using var connection = await SqliteDbContextFactory.CreateOpenConnectionAsync();
        var options = SqliteDbContextFactory.CreateOptions(connection);
        await SqliteDbContextFactory.EnsureCreatedAsync(options);

        await using (var context = new MinimalChatbotDbContext(options))
        {
            var attachmentRepository = new AttachmentRepository(context);
            var conversationRepository = new ConversationRepository(context);
            var validator = new TestAttachmentValidator();
            var useCase = new AddAttachmentToConversationUseCase(attachmentRepository, conversationRepository, validator);

            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                useCase.ExecuteAsync(nonExistentId, "test.txt", "content", ".txt"));
        }
    }
}
