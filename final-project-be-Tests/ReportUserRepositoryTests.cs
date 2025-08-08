using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Tests
{
    public static class LoggerExtensionsForTests
    {
        public static void VerifyLog<T>(this Mock<ILogger<T>> logger, LogLevel level, string message, Times times)
        {
            logger.Verify(
                l => l.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(message)),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                times
            );
        }
    }

    public class ReportUserRepositoryTests
    {
        private readonly Mock<IReportUserDAO> _mockReportUserDAO;
        private readonly Mock<IReportDAO> _mockReportDAO;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<ReportUserRepository>> _mockLogger;
        private readonly ReportUserRepository _repository;

        public ReportUserRepositoryTests()
        {
            _mockReportUserDAO = new Mock<IReportUserDAO>();
            _mockReportDAO = new Mock<IReportDAO>();
            _mockLogger = new Mock<ILogger<ReportUserRepository>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ReportUserDto, Report>();
                cfg.CreateMap<ReportUserDto, ReportUser>();
            });
            _mapper = mapperConfig.CreateMapper();

            _repository = new ReportUserRepository(
                _mockReportUserDAO.Object,
                _mapper,
                _mockLogger.Object,
                _mockReportDAO.Object
            );
        }

        [Fact]
        public async Task CreateReportUser_Success_ReturnsReportUser()
        {
            // Arrange
            var dto = new ReportUserDto();
            var report = new Report { ReportId = 1 };

            _mockReportDAO
                .Setup(d => d.AddAsync(It.IsAny<Report>()))
                .Callback<Report>(r => r.ReportId = 1)
                .Returns(Task.CompletedTask);

            _mockReportUserDAO
                .Setup(d => d.AddAsync(It.IsAny<ReportUser>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateReportUser(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ReportId);
            _mockReportUserDAO.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _mockReportUserDAO.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, "AddAsync ReportComment success", Times.Once());
        }

        [Fact]
        public async Task CreateReportUser_ReportIdInvalid_ReturnsNull()
        {
            // Arrange
            var dto = new ReportUserDto();

            _mockReportDAO
                .Setup(d => d.AddAsync(It.IsAny<Report>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateReportUser(dto);

            // Assert
            Assert.Null(result);
            _mockReportUserDAO.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Error, "Failed to create Report, cannot proceed with ReportComment.", Times.Once());
        }

        [Fact]
        public async Task CreateReportUser_ExceptionThrown_ReturnsNull()
        {
            // Arrange
            var dto = new ReportUserDto();

            _mockReportDAO
                .Setup(d => d.AddAsync(It.IsAny<Report>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _repository.CreateReportUser(dto);

            // Assert
            Assert.Null(result);
            _mockReportUserDAO.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Error, "Error when adding ReportComment", Times.Once());
        }

        [Fact]
        public void GetAllReportUsers_Success_ReturnsPageResult()
        {
            // Arrange
            var reportUsers = new List<ReportUser>
        {
            new ReportUser { ReportId = 1, UserId = Guid.NewGuid() },
            new ReportUser { ReportId = 2, UserId = Guid.NewGuid() }
        }.AsQueryable();

            _mockReportUserDAO.Setup(d => d.GetAll()).Returns(reportUsers);

            // Act
            var result = _repository.GetAllReportUsers(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            _mockLogger.VerifyLog(LogLevel.Information, "Get ReportComments success", Times.Once());
        }

        [Fact]
        public void GetAllReportUsers_Exception_ReturnsEmptyResult()
        {
            // Arrange
            _mockReportUserDAO.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            // Act
            var result = _repository.GetAllReportUsers(1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
            _mockLogger.VerifyLog(LogLevel.Error, "Error when getting ReportComments", Times.Once());
        }

        [Fact]
        public async Task GetReportUser_Success_ReturnsReportUser()
        {
            // Arrange
            var expectedReportUser = new ReportUser { ReportId = 1, UserId = Guid.NewGuid() };

            _mockReportUserDAO.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockReportUserDAO.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _mockReportUserDAO.Setup(d => d.GetByReportId(1)).Returns(expectedReportUser);

            // Act
            var result = await _repository.GetReportUser(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedReportUser.ReportId, result.ReportId);
            Assert.Equal(expectedReportUser.UserId, result.UserId);

            _mockReportUserDAO.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _mockReportUserDAO.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Information, "Get ReportComment success", Times.Once());
        }

        [Fact]
        public async Task GetReportUser_Exception_ReturnsNull()
        {
            // Arrange
            _mockReportUserDAO.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockReportUserDAO.Setup(d => d.GetByReportId(It.IsAny<int>())).Throws(new Exception("DB error"));
            _mockReportUserDAO.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.GetReportUser(99);

            // Assert
            Assert.Null(result);

            _mockReportUserDAO.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Error, "Error when get ReportComment", Times.Once());
        }

        [Fact]
        public async Task DeleteReportsByUserId_HasData_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var reportUsers = new List<ReportUser>
        {
            new ReportUser { ReportId = 1, UserId = userId },
            new ReportUser { ReportId = 2, UserId = userId }
        };

            _mockReportDAO.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockReportUserDAO.Setup(d => d.GetByUserId(userId)).Returns(reportUsers);
            _mockReportUserDAO.Setup(d => d.DeleteByReportAndUserId(It.IsAny<int>(), It.IsAny<Guid>()));
            _mockReportDAO.Setup(d => d.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            _mockReportDAO.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.DeleteReportsByUserId(userId);

            // Assert
            Assert.True(result);
            _mockReportUserDAO.Verify(d => d.DeleteByReportAndUserId(It.IsAny<int>(), userId), Times.Exactly(reportUsers.Count));
            _mockReportDAO.Verify(d => d.DeleteAsync(It.IsAny<int>()), Times.Exactly(reportUsers.Count));
            _mockLogger.VerifyLog(LogLevel.Information, $"Successfully deleted all reports for userId: {userId}", Times.Once());
        }

        [Fact]
        public async Task DeleteReportsByUserId_NoData_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockReportDAO.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _mockReportUserDAO.Setup(d => d.GetByUserId(userId)).Returns(new List<ReportUser>());
            _mockReportDAO.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.DeleteReportsByUserId(userId);

            // Assert
            Assert.False(result);
            _mockLogger.VerifyLog(LogLevel.Warning, $"No reports found for userId: {userId}", Times.Once());
            _mockReportDAO.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteReportsByUserId_Exception_ReturnsFalse()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mockReportDAO.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _mockReportDAO.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.DeleteReportsByUserId(userId);

            // Assert
            Assert.False(result);
            _mockReportDAO.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            _mockLogger.VerifyLog(LogLevel.Error, $"Error deleting reports for userId: {userId}", Times.Once());
        }
    }
}
