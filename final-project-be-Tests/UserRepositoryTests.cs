using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Users;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace final_project_be_Tests
{
    public class UserRepositoryTests
    {
        private readonly Mock<IUserDAO> _userDaoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<UserRepository>> _loggerMock = new();
        private readonly UserRepository _repository;

        public UserRepositoryTests()
        {
            _repository = new UserRepository(
                _userDaoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task ToggleIsBanned_ShouldToggleAndReturnUser_WhenFound()
        {
            var userId = Guid.NewGuid();
            var user = new User { UserId = userId, IsBanned = false };
            _userDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(user);
            _userDaoMock.Setup(d => d.UpdateAsync(user)).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.ToggleIsBanned(userId);

            Assert.NotNull(result);
            Assert.True(result.IsBanned);
            _userDaoMock.Verify(d => d.UpdateAsync(user), Times.Once);
            _userDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task ToggleIsBanned_ShouldReturnNull_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            _userDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync((User)null);

            var result = await _repository.ToggleIsBanned(userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task ToggleIsBanned_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var userId = Guid.NewGuid();
            _userDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).Throws(new Exception("DB error"));
            _userDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.ToggleIsBanned(userId);

            Assert.Null(result);
            _userDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetAllUsers_ShouldReturnPagedResult()
        {
            var users = new List<User>
            {
                new User { UserId = Guid.NewGuid(), UserMetaData = new UserMetadata() },
                new User { UserId = Guid.NewGuid(), UserMetaData = new UserMetadata() }
            }.AsQueryable();

            var dbSetMock = new Mock<DbSet<User>>();
            _userDaoMock.Setup(d => d.GetAll()).Returns(users);

            var result = _repository.GetAllUsers(1, 2);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
        }

        [Fact]
        public void GetAllUsers_ShouldReturnEmptyResult_WhenExceptionThrown()
        {
            _userDaoMock.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            var result = _repository.GetAllUsers(1, 2);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetUserandUserMetadata_ShouldReturnUser_WhenFound()
        {
            var userId = Guid.NewGuid();
            var user = new User { UserId = userId, UserMetaData = new UserMetadata() };
            _userDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(user);
            _userDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetUserandUserMetadata(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
        }

        [Fact]
        public async Task GetUserandUserMetadata_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var userId = Guid.NewGuid();
            _userDaoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _userDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetUserandUserMetadata(userId);

            Assert.Null(result);
            _userDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetUserStatisticsByMonth_ShouldReturnStats()
        {
            var users = new List<User>
            {
                new User { UserId = Guid.NewGuid(), CreateAt = new DateTime(2024, 1, 1) },
                new User { UserId = Guid.NewGuid(), CreateAt = new DateTime(2024, 1, 2) },
                new User { UserId = Guid.NewGuid(), CreateAt = new DateTime(2024, 2, 1) }
            }.AsQueryable();

            _userDaoMock.Setup(d => d.GetAll()).Returns(users);

            var result = _repository.GetUserStatisticsByMonth();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, x => x.Month == "01/2024" && x.Total == 2);
            Assert.Contains(result, x => x.Month == "02/2024" && x.Total == 1);
        }

        [Fact]
        public void GetUserStatisticsByMonth_ShouldReturnEmptyList_WhenExceptionThrown()
        {
            _userDaoMock.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            var result = _repository.GetUserStatisticsByMonth();

            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateUserPoint_ShouldReturnUser_WhenFound()
        {
            var userId = Guid.NewGuid();
            var user = new User { UserId = userId, Point = 10 };
            _userDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(user);
            _userDaoMock.Setup(d => d.UpdateAsync(user)).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateUserPoint(5, userId);

            Assert.NotNull(result);
            Assert.Equal(15, result.Point);
            _userDaoMock.Verify(d => d.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateUserPoint_ShouldReturnNullAndRollback_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            _userDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync((User)null);
            _userDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateUserPoint(5, userId);

            Assert.Null(result);
            _userDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserPoint_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var userId = Guid.NewGuid();
            _userDaoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _userDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateUserPoint(5, userId);

            Assert.Null(result);
            _userDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateMetadataAsync_ShouldReturnTrue_WhenMetadataFound()
        {
            var userId = Guid.NewGuid();
            var metadata = new UserMetadata { UserId = userId, FirstName = "A" };
            var dto = new UpdateUserMetadataDto { FirstName = "B" };
            _userDaoMock.Setup(d => d.GetUserMetadatabyId(userId)).ReturnsAsync(metadata);
            _userDaoMock.Setup(d => d.UpdateUserMetadataAsync(metadata)).Returns(Task.CompletedTask);

            var result = await _repository.UpdateMetadataAsync(userId, dto);

            Assert.True(result);
            Assert.Equal("B", metadata.FirstName);
            _userDaoMock.Verify(d => d.UpdateUserMetadataAsync(metadata), Times.Once);
        }

        [Fact]
        public async Task UpdateMetadataAsync_ShouldReturnFalse_WhenMetadataNotFound()
        {
            var userId = Guid.NewGuid();
            var dto = new UpdateUserMetadataDto { FirstName = "B" };
            _userDaoMock.Setup(d => d.GetUserMetadatabyId(userId)).ReturnsAsync((UserMetadata)null);

            var result = await _repository.UpdateMetadataAsync(userId, dto);

            Assert.False(result);
        }

        [Fact]
        public async Task GetUserProfileSummaryAsync_ShouldReturnSummary_WhenUserAndMetadataFound()
        {
            var userId = Guid.NewGuid();
            var meta = new UserMetadata
            {
                Birthday = new DateTime(2000, 1, 1),
                Nationality = "Vietnamese",
                Level = "Advanced",
                Goals = "become fluent",
                FavouriteSubject = "Math"
            };
            var user = new User { UserId = userId, UserMetaData = meta };
            var users = new List<User> { user }.AsQueryable();

            var dbSetMock = new Mock<DbSet<User>>();
            _userDaoMock.Setup(d => d.GetAll()).Returns(users);

            var result = await _repository.GetUserProfileSummaryAsync(userId);

            Assert.Contains("year-old", result);
            Assert.Contains("Vietnamese", result);
            Assert.Contains("advanced", result);
            Assert.Contains("become fluent", result);
            Assert.Contains("Math", result);
        }

        [Fact]
        public async Task GetUserProfileSummaryAsync_ShouldReturnUnknown_WhenUserOrMetadataNotFound()
        {
            var userId = Guid.NewGuid();
            var users = new List<User>().AsQueryable();
            _userDaoMock.Setup(d => d.GetAll()).Returns(users);

            var result = await _repository.GetUserProfileSummaryAsync(userId);

            Assert.Equal("Unknown user with no metadata", result);
        }
    }
}