using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Notification;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace final_project_be_Tests
{
    public class NotificationRepositoryTests
    {
        private readonly Mock<INotificationDAO> _daoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<NotificationRepository>> _loggerMock = new();
        private readonly Mock<IHubContext<SignalRHub>> _signalrHubMock = new();
        private readonly NotificationRepository _repository;

        public NotificationRepositoryTests()
        {
            _repository = new NotificationRepository(
                _daoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _signalrHubMock.Object
            );
        }

        [Fact]
        public async Task CreateNotification_ShouldReturnNotification_WhenSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var dto = new NotificationDto { NotificationId = 1, UserId = userId, Message = "Test" };
            var notification = new Notification { NotificationId = 1, UserId = userId, Message = "Test" };

            _mapperMock.Setup(m => m.Map<Notification>(dto)).Returns(notification);
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.AddAsync(notification)).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Mock HubContext
            var clientProxyMock = new Mock<IClientProxy>();
            clientProxyMock
                .Setup(c => c.SendCoreAsync(
                    "ReceiveNotification",
                    It.IsAny<object[]>(),
                    default
                )).Returns(Task.CompletedTask);

            var clientsMock = new Mock<IHubClients>();
            clientsMock.Setup(c => c.User(userId.ToString())).Returns(clientProxyMock.Object);

            _signalrHubMock.Setup(h => h.Clients).Returns(clientsMock.Object);

            // Act
            var result = await _repository.CreateNotification(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Message);

            _daoMock.Verify(d => d.AddAsync(notification), Times.Once);
            _daoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
            clientProxyMock.Verify(c => c.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(o => o.Length == 1),
                default
            ), Times.Once);
        }

        [Fact]
        public async Task CreateNotification_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var dto = new NotificationDto { NotificationId = 1, UserId = Guid.NewGuid(), Message = "Test" };
            _mapperMock.Setup(m => m.Map<Notification>(dto)).Throws(new Exception("Mapping failed"));
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateNotification(dto);

            Assert.Null(result);
            _daoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteNotification_ShouldReturnTrue_WhenSuccess()
        {
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.DeleteAsync(1)).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteNotification(1);

            Assert.True(result);
            _daoMock.Verify(d => d.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteNotification_ShouldReturnFalseAndRollback_WhenExceptionThrown()
        {
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.DeleteAsync(It.IsAny<int>())).Throws(new Exception("Delete failed"));
            _daoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteNotification(1);

            Assert.False(result);
            _daoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetAllNotifications_ShouldReturnPagedResult()
        {
            var data = new List<Notification>
            {
                new Notification { NotificationId = 1, Message = "A" },
                new Notification { NotificationId = 2, Message = "B" }
            }.AsQueryable();

            _daoMock.Setup(d => d.GetAll()).Returns(data);

            var result = _repository.GetAllNotifications(1, 2);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
        }

        [Fact]
        public void GetAllNotifications_ShouldReturnEmptyResult_WhenExceptionThrown()
        {
            _daoMock.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            var result = _repository.GetAllNotifications(1, 2);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetNotification_ShouldReturnNotification_WhenFound()
        {
            var notification = new Notification { NotificationId = 1, Message = "A" };
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(notification);
            _daoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetNotification(1);

            Assert.NotNull(result);
            Assert.Equal("A", result.Message);
        }

        [Fact]
        public async Task GetNotification_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            _daoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _daoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetNotification(1);

            Assert.Null(result);
            _daoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task GetNotificationsByUser_ShouldReturnNotifications_WhenSuccess()
        {
            var userId = Guid.NewGuid();
            var data = new List<Notification>
            {
                new Notification { NotificationId = 1, UserId = userId, Message = "A" },
                new Notification { NotificationId = 2, UserId = userId, Message = "B" }
            }.AsQueryable();

            _daoMock.Setup(d => d.GetAll()).Returns(data);

            var result = await _repository.GetNotificationsByUser(userId);

            Assert.Equal(2, result.Count);
            Assert.All(result, n => Assert.Equal(userId, n.UserId));
        }

        [Fact]
        public async Task GetNotificationsByUser_ShouldReturnEmptyList_WhenExceptionThrown()
        {
            var userId = Guid.NewGuid();
            _daoMock.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            var result = await _repository.GetNotificationsByUser(userId);

            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateNotification_ShouldReturnNotification_WhenSuccess()
        {
            var dto = new NotificationDto { NotificationId = 1, UserId = Guid.NewGuid(), Message = "Updated" };
            var notification = new Notification { NotificationId = 1, UserId = dto.UserId, Message = "Updated" };
            _mapperMock.Setup(m => m.Map<Notification>(dto)).Returns(notification);
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.UpdateAsync(notification)).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateNotification(dto);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.Message);
            _daoMock.Verify(d => d.UpdateAsync(notification), Times.Once);
        }

        [Fact]
        public async Task UpdateNotification_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var dto = new NotificationDto { NotificationId = 1, UserId = Guid.NewGuid(), Message = "Updated" };
            _mapperMock.Setup(m => m.Map<Notification>(dto)).Throws(new Exception("Mapping failed"));
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateNotification(dto);

            Assert.Null(result);
            _daoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }
    }
}