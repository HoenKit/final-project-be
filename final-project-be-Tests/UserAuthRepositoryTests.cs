using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Application.Service.EmailService;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Users;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace final_project_be_Tests
{
    public class UserAuthRepositoryTests
    {
        private readonly Mock<IUserDAO> _userDaoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<UserAuthRepository>> _loggerMock = new();
        private readonly Mock<IConfiguration> _configMock = new();
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock = new();
        private readonly Mock<IEmailService> _emailServiceMock = new();
        private readonly IOptions<ClientSettings> _clientSettings = Options.Create(new ClientSettings { BaseUrl = "http://localhost/" });
        private readonly IOptions<GoogleSettings> _googleSettings = Options.Create(new GoogleSettings { ClientId = "id", ClientSecret = "secret", RedirectUri = "uri" });

        private readonly UserAuthRepository _repository;

        public UserAuthRepositoryTests()
        {
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext());
            _repository = new UserAuthRepository(
                _userDaoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _configMock.Object,
                _httpContextAccessorMock.Object,
                _emailServiceMock.Object,
                _clientSettings,
                _googleSettings
            );
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenUserNotFound()
        {
            var dto = new UserLoginDto { Email = "test@example.com", Password = "pass" };
            _userDaoMock.Setup(d => d.GetUserbyEmail(dto.Email)).Returns((User)null);

            var result = await _repository.LoginAsync(dto);

            Assert.False(result.Success);
            Assert.Contains("No account", result.ErrorMessage);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenUserIsBanned()
        {
            var dto = new UserLoginDto { Email = "test@example.com", Password = "pass" };
            var user = new User { IsBanned = true };
            _userDaoMock.Setup(d => d.GetUserbyEmail(dto.Email)).Returns(user);

            var result = await _repository.LoginAsync(dto);

            Assert.False(result.Success);
            Assert.Contains("banned", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenUserNotConfirmed()
        {
            var dto = new UserLoginDto { Email = "test@example.com", Password = "pass" };
            var user = new User { IsBanned = false, IsConfirmed = false };
            _userDaoMock.Setup(d => d.GetUserbyEmail(dto.Email)).Returns(user);

            var result = await _repository.LoginAsync(dto);

            Assert.False(result.Success);
            Assert.Contains("not been confirmed", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnError_WhenPasswordIncorrect()
        {
            var dto = new UserLoginDto { Email = "test@example.com", Password = "wrongpass" };
            var user = new User { IsBanned = false, IsConfirmed = true, Password = "hashed" };
            _userDaoMock.Setup(d => d.GetUserbyEmail(dto.Email)).Returns(user);

            // PasswordHasher will always fail for wrong plain text
            var result = await _repository.LoginAsync(dto);

            Assert.False(result.Success);
            Assert.Contains("unexpected error", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnNull_WhenUserExists()
        {
            var dto = new UserRegisterDto { userMetadataDto = new UserMetadataDto { FirstName = "A", LastName = "B" } };
            _userDaoMock.Setup(d => d.UserRegisterExist(dto)).Returns(true);

            var result = await _repository.RegisterAsync(dto);

            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnUser_WhenSuccess()
        {
            var dto = new UserRegisterDto { userMetadataDto = new UserMetadataDto { FirstName = "A", LastName = "B" }, Email = "test@example.com", Password = "pass" };
            var user = new User { UserId = Guid.NewGuid(), Email = dto.Email };
            var role = new Role { RoleId = 1, RoleName = "User" };
            _userDaoMock.Setup(d => d.UserRegisterExist(dto)).Returns(false);
            _mapperMock.Setup(m => m.Map<User>(dto)).Returns(user);
            _userDaoMock.Setup(d => d.AddAsync(user)).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.GetRoleByNameAsync("User")).ReturnsAsync(role);
            _userDaoMock.Setup(d => d.AddUserMetaData(It.IsAny<UserMetadata>())).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.AddUserRoleAsync(It.IsAny<UserRole>())).Returns(Task.CompletedTask);
            _emailServiceMock.Setup(e => e.SendEmailAsync(dto.Email, It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.RegisterAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Email, result.Email);
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var dto = new UserRegisterDto { userMetadataDto = new UserMetadataDto { FirstName = "A", LastName = "B" }, Email = "test@example.com", Password = "pass" };
            _userDaoMock.Setup(d => d.UserRegisterExist(dto)).Returns(false);
            _mapperMock.Setup(m => m.Map<User>(dto)).Throws(new Exception("Mapping failed"));
            _userDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.RegisterAsync(dto);

            Assert.Null(result);
            _userDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task ConfirmAccountAsync_ShouldReturnNull_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync((User)null);
            _userDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.ConfirmAccountAsync(userId);

            Assert.Null(result);
        }

        [Fact]
        public async Task ConfirmAccountAsync_ShouldReturnUser_WhenSuccess()
        {
            var userId = Guid.NewGuid();
            var user = new User { UserId = userId, IsConfirmed = false };
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).ReturnsAsync(user);
            _userDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.UpdateAsync(user)).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.ConfirmAccountAsync(userId);

            Assert.NotNull(result);
            Assert.True(result.IsConfirmed);
        }

        [Fact]
        public async Task ConfirmAccountAsync_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var userId = Guid.NewGuid();
            _userDaoMock.Setup(d => d.GetByIdAsync(userId)).Throws(new Exception("DB error"));
            _userDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _userDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.ConfirmAccountAsync(userId);

            Assert.Null(result);
            _userDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }
    }
}