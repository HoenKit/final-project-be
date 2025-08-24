using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Message;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace final_project_be_Tests
{
    public class MessagesRepositoryTests
    {
        private readonly Mock<IMessageDAO> _messageDaoMock;
        private readonly Mock<ILogger<MessagesRepository>> _loggerMock;
        private readonly MessagesRepository _repository;

        public MessagesRepositoryTests()
        {
            _messageDaoMock = new Mock<IMessageDAO>();
            _loggerMock = new Mock<ILogger<MessagesRepository>>();
            _repository = new MessagesRepository(_messageDaoMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetConversationAsync_ShouldReturnMessages()
        {
            var userId = Guid.NewGuid();
            var mentorUserId = Guid.NewGuid();
            var messages = new List<Messages>
            {
                new Messages { MessageId = 1, SenderId = userId, ReceiverId = mentorUserId, Content = "Hi" }
            };
            _messageDaoMock.Setup(d => d.GetMessagesAsync(userId, mentorUserId)).ReturnsAsync(messages);

            var result = await _repository.GetConversationAsync(userId, mentorUserId);

            Assert.Single(result);
            Assert.Equal("Hi", result[0].Content);
        }

        [Fact]
        public async Task SendMessageAsync_ShouldReturnMessage()
        {
            var senderId = Guid.NewGuid();
            var receiverId = Guid.NewGuid();
            var content = "Hello";
            var message = new Messages { MessageId = 1, SenderId = senderId, ReceiverId = receiverId, Content = content };
            _messageDaoMock.Setup(d => d.SendMessageAsync(It.IsAny<Messages>())).ReturnsAsync(message);

            var result = await _repository.SendMessageAsync(senderId, receiverId, content);

            Assert.NotNull(result);
            Assert.Equal(content, result.Content);
            Assert.Equal(senderId, result.SenderId);
            Assert.Equal(receiverId, result.ReceiverId);
        }

        [Fact]
        public async Task GetChatPartnersAsync_ShouldReturnChatPartners()
        {
            var userId = Guid.NewGuid();
            var partnerId = Guid.NewGuid();
            var sentMessages = new List<(Guid PartnerId, DateTime SentAt)>
            {
                (partnerId, DateTime.UtcNow.AddMinutes(-5))
            };
            var receivedMessages = new List<(Guid PartnerId, DateTime SentAt)>
            {
                (partnerId, DateTime.UtcNow.AddMinutes(-2))
            };
            var users = new List<User>
            {
                new User
                {
                    UserId = partnerId,
                    UserMetaData = new UserMetadata { FirstName = "John", LastName = "Doe", Avatar = "avatar.png" }
                }
            };

            _messageDaoMock.Setup(d => d.GetSentMessagesAsync(userId)).ReturnsAsync(sentMessages);
            _messageDaoMock.Setup(d => d.GetReceivedMessagesAsync(userId)).ReturnsAsync(receivedMessages);
            _messageDaoMock.Setup(d => d.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(users);

            var result = await _repository.GetChatPartnersAsync(userId);

            Assert.Single(result);
            Assert.Equal(partnerId, result[0].UserId);
            Assert.Equal("JohnDoe", result[0].FullName);
            Assert.Equal("avatar.png", result[0].Avatar);
        }

        [Fact]
        public async Task GetChatPartnersAsync_ShouldReturnEmpty_WhenNoMessages()
        {
            var userId = Guid.NewGuid();
            _messageDaoMock.Setup(d => d.GetSentMessagesAsync(userId)).ReturnsAsync(new List<(Guid, DateTime)>());
            _messageDaoMock.Setup(d => d.GetReceivedMessagesAsync(userId)).ReturnsAsync(new List<(Guid, DateTime)>());
            _messageDaoMock.Setup(d => d.GetUsersByIdsAsync(It.IsAny<IEnumerable<Guid>>())).ReturnsAsync(new List<User>());

            var result = await _repository.GetChatPartnersAsync(userId);

            Assert.Empty(result);
        }

        [Fact]
        public async Task IsNewChatRoomAsync_ShouldReturnTrue_WhenNoMessages()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            _messageDaoMock.Setup(d => d.GetMessagesAsync(userId1, userId2)).ReturnsAsync(new List<Messages>());

            var result = await _repository.IsNewChatRoomAsync(userId1, userId2);

            Assert.True(result);
        }

        [Fact]
        public async Task IsNewChatRoomAsync_ShouldReturnFalse_WhenMessagesExist()
        {
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            var messages = new List<Messages>
            {
                new Messages { MessageId = 1, SenderId = userId1, ReceiverId = userId2, Content = "Hi" }
            };
            _messageDaoMock.Setup(d => d.GetMessagesAsync(userId1, userId2)).ReturnsAsync(messages);

            var result = await _repository.IsNewChatRoomAsync(userId1, userId2);

            Assert.False(result);
        }
    }
}