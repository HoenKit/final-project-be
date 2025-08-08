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
    public class ReportPostRepositoryTests
    {
        private readonly Mock<IReportPostDAO> _mockReportPostDAO;
        private readonly Mock<IReportDAO> _mockReportDAO;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<ReportPostRepository>> _mockLogger;
        private readonly ReportPostRepository _repository;

        public ReportPostRepositoryTests()
        {
            _mockReportPostDAO = new Mock<IReportPostDAO>();
            _mockReportDAO = new Mock<IReportDAO>();
            _mockLogger = new Mock<ILogger<ReportPostRepository>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ReportPostDto, Report>();
                cfg.CreateMap<ReportPostDto, ReportPost>();
            });
            _mapper = config.CreateMapper();

            _repository = new ReportPostRepository(
                _mockReportPostDAO.Object,
                _mapper,
                _mockLogger.Object,
                _mockReportDAO.Object
            );
        }

        [Fact]
        public async Task CreateReportPost_Success_ReturnsReportPost()
        {
            // Arrange
            var dto = new ReportPostDto
            {
                ReportId = 0,
                PostId = 10,
                UserId = Guid.NewGuid(),
                Content = "Test content"
            };

            var report = new Report { ReportId = 1 };
            _mockReportDAO
                .Setup(d => d.AddAsync(It.IsAny<Report>()))
                .Callback<Report>(r => r.ReportId = report.ReportId)
                .Returns(Task.CompletedTask);

            _mockReportPostDAO
                .Setup(d => d.AddAsync(It.IsAny<ReportPost>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateReportPost(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ReportId);
            _mockReportPostDAO.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _mockReportPostDAO.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _mockReportDAO.Verify(d => d.AddAsync(It.IsAny<Report>()), Times.Once);
            _mockReportPostDAO.Verify(d => d.AddAsync(It.IsAny<ReportPost>()), Times.Once);
        }

        [Fact]
        public async Task CreateReportPost_FailReportCreation_ReturnsNull()
        {
            // Arrange
            var dto = new ReportPostDto();
            _mockReportDAO
                .Setup(d => d.AddAsync(It.IsAny<Report>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateReportPost(dto);

            // Assert
            Assert.Null(result);
            _mockReportPostDAO.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateReportPost_ExceptionThrown_ReturnsNull()
        {
            // Arrange
            var dto = new ReportPostDto();
            _mockReportDAO
                .Setup(d => d.AddAsync(It.IsAny<Report>()))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _repository.CreateReportPost(dto);

            // Assert
            Assert.Null(result);
            _mockReportPostDAO.Verify(d => d.RollbackTransactionAsync(), Times.Once);

            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error when adding ReportPost")),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()
                ),
                Times.Once
            );
        }

        [Fact]
        public void GetAllReportPosts_ReturnsPagedResult_WhenSuccess()
        {
            // Arrange
            var data = new List<ReportPost>
    {
        new ReportPost { ReportId = 1 },
        new ReportPost { ReportId = 2 },
        new ReportPost { ReportId = 3 }
    }.AsQueryable();

            _mockReportPostDAO.Setup(d => d.GetAll()).Returns(data);

            // Act
            var result = _repository.GetAllReportPosts(1, 2);

            // Assert
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.PageSize);

            _mockLogger.Verify(
                log => log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Get ReportPosts success")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }

        [Fact]
        public void GetAllReportPosts_ReturnsEmptyResult_WhenExceptionThrown()
        {
            // Arrange
            _mockReportPostDAO.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            // Act
            var result = _repository.GetAllReportPosts(1, 2);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.PageSize);

            // Verify log lỗi
            _mockLogger.Verify(
                log => log.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error when getting ReportPosts")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task GetReportPost_ReturnsReportPost_WhenSuccess()
        {
            // Arrange
            var reportPost = new ReportPost { ReportId = 1 };
            _mockReportPostDAO
                .Setup(d => d.GetByReportId(1))
                .Returns(reportPost);

            // Act
            var result = await _repository.GetReportPost(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ReportId);

            _mockReportPostDAO.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _mockReportPostDAO.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _mockReportPostDAO.Verify(d => d.RollbackTransactionAsync(), Times.Never);

            // Verify log thông tin
            _mockLogger.Verify(
                log => log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Get ReportPost success")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task GetReportPost_ReturnsNull_WhenExceptionThrown()
        {
            // Arrange
            _mockReportPostDAO
                .Setup(d => d.GetByReportId(It.IsAny<int>()))
                .Throws(new Exception("DB error"));

            // Act
            var result = await _repository.GetReportPost(1);

            // Assert
            Assert.Null(result);

            _mockReportPostDAO.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _mockReportPostDAO.Verify(d => d.CommitTransactionAsync(), Times.Never);
            _mockReportPostDAO.Verify(d => d.RollbackTransactionAsync(), Times.Once);

            // Verify log lỗi
            _mockLogger.Verify(
                log => log.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error when get ReportPost")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteReportsByPostId_NoReportsFound_ReturnsFalse()
        {
            // Arrange
            _mockReportPostDAO
                .Setup(d => d.GetByPostId(1))
                .Returns((List<ReportPost>)null);

            // Act
            var result = await _repository.DeleteReportsByPostId(1);

            // Assert
            Assert.False(result);

            _mockReportDAO.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _mockReportDAO.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _mockReportDAO.Verify(d => d.RollbackTransactionAsync(), Times.Never);

            _mockLogger.Verify(
                log => log.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No reports found for postId")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteReportsByPostId_Success_ReturnsTrue()
        {
            // Arrange
            var reportPosts = new List<ReportPost>
    {
        new ReportPost { ReportId = 1, PostId = 1 },
        new ReportPost { ReportId = 2, PostId = 1 }
    };

            _mockReportPostDAO
                .Setup(d => d.GetByPostId(1))
                .Returns(reportPosts);

            // Act
            var result = await _repository.DeleteReportsByPostId(1);

            // Assert
            Assert.True(result);

            _mockReportDAO.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _mockReportDAO.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _mockReportDAO.Verify(d => d.RollbackTransactionAsync(), Times.Never);

            _mockReportPostDAO.Verify(d => d.DeleteByReportAndPostId(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(reportPosts.Count));
            _mockReportDAO.Verify(d => d.DeleteAsync(It.IsAny<int>()), Times.Exactly(reportPosts.Count));

            _mockLogger.Verify(
                log => log.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Successfully deleted all reports for postId")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteReportsByPostId_ExceptionThrown_ReturnsFalse()
        {
            // Arrange
            _mockReportPostDAO
                .Setup(d => d.GetByPostId(It.IsAny<int>()))
                .Throws(new Exception("DB error"));

            // Act
            var result = await _repository.DeleteReportsByPostId(1);

            // Assert
            Assert.False(result);

            _mockReportDAO.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _mockReportDAO.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            _mockReportDAO.Verify(d => d.CommitTransactionAsync(), Times.Never);

            _mockLogger.Verify(
                log => log.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error deleting reports for postId")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }

    }
}
