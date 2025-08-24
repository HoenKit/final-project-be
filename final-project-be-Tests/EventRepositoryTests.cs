using System;
using System.Threading.Tasks;
using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace final_project_be_Tests
{
    public class EventRepositoryTests
    {
        private readonly Mock<IUserDAO> _userDaoMock;
        private readonly Mock<IEventDAO> _eventDaoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<EventRepository>> _loggerMock;
        private readonly EventRepository _repository;

        public EventRepositoryTests()
        {
            _userDaoMock = new Mock<IUserDAO>();
            _eventDaoMock = new Mock<IEventDAO>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<EventRepository>>();
            _repository = new EventRepository(
                _userDaoMock.Object,
                _eventDaoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task AddPointsAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var (success, message, user) = await _repository.AddPointsAsync(userId, 10);

            Assert.False(success);
            Assert.Equal("User not found", message);
            Assert.Null(user);
        }

        [Fact]
        public async Task AddPointsAsync_ShouldReturnFalse_WhenNoTurnsLeft()
        {
            var userId = Guid.NewGuid();
            var user = new User { Turns = 0, Point = 5 };
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(user);

            var (success, message, resultUser) = await _repository.AddPointsAsync(userId, 10);

            Assert.False(success);
            Assert.Equal("No turns left", message);
            Assert.Equal(user, resultUser);
        }

        [Fact]
        public async Task AddPointsAsync_ShouldAddPointsAndDeductTurn_WhenValid()
        {
            var userId = Guid.NewGuid();
            var user = new User { Turns = 2, Point = 5 };
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(user);
            _userDaoMock.Setup(d => d.UpdateAsync(user)).Returns(Task.CompletedTask);

            var (success, message, resultUser) = await _repository.AddPointsAsync(userId, 10);

            Assert.True(success);
            Assert.Equal("Points added and turn deducted", message);
            Assert.Equal(15, user.Point);
            Assert.Equal(1, user.Turns);
            _userDaoMock.Verify(d => d.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task DailyLoginAsync_ShouldReturnFalse_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var (success, message, user) = await _repository.DailyLoginAsync(userId);

            Assert.False(success);
            Assert.Equal("User not found", message);
            Assert.Null(user);
        }

        [Fact]
        public async Task DailyLoginAsync_ShouldReturnFalse_WhenAlreadyLoggedInToday()
        {
            var userId = Guid.NewGuid();
            var today = DateTime.UtcNow.Date;
            var user = new User { LastDailyLogin = today, Turns = 1 };
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(user);

            var (success, message, resultUser) = await _repository.DailyLoginAsync(userId);

            Assert.False(success);
            Assert.Equal("Already logged in today", message);
            Assert.Equal(user, resultUser);
        }

        [Fact]
        public async Task DailyLoginAsync_ShouldIncreaseTurnsAndSetLastLogin_WhenFirstLoginToday()
        {
            var userId = Guid.NewGuid();
            var yesterday = DateTime.UtcNow.AddDays(-1);
            var user = new User { LastDailyLogin = yesterday, Turns = 1 };
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(user);
            _userDaoMock.Setup(d => d.UpdateAsync(user)).Returns(Task.CompletedTask);

            var (success, message, resultUser) = await _repository.DailyLoginAsync(userId);

            Assert.True(success);
            Assert.Equal("Daily login successful", message);
            Assert.Equal(2, user.Turns);
            Assert.True(user.LastDailyLogin.HasValue);
            Assert.Equal(user, resultUser);
            _userDaoMock.Verify(d => d.UpdateAsync(user), Times.Once);
        }
    }
}