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
    public class ReportCommentRepositoryTests
    {
        private readonly Mock<IReportCommentDAO> _reportCommentDaoMock = new();
        private readonly Mock<IReportDAO> _reportDaoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<ReportCommentRepository>> _loggerMock = new();

        private ReportCommentRepository CreateRepository()
        {
            return new ReportCommentRepository(
                _reportCommentDaoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _reportDaoMock.Object
            );
        }

        [Fact]
        public async Task CreateReportComment_ShouldCreateSuccessfully()
        {
            // Arrange
            var dto = new ReportCommentDto
            {
                Content = "Test comment",
                UserId = Guid.NewGuid()
            };
            var mappedReport = new Report { ReportId = 123 };
            var mappedReportComment = new ReportComment { CommentId = 456 };

            _mapperMock.Setup(m => m.Map<Report>(dto)).Returns(mappedReport);
            _reportDaoMock.Setup(d => d.AddAsync(It.IsAny<Report>()))
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<ReportComment>(It.IsAny<ReportCommentDto>())).Returns(mappedReportComment);
            _reportCommentDaoMock.Setup(d => d.AddAsync(It.IsAny<ReportComment>()))
                .Returns(Task.CompletedTask);


            var repo = CreateRepository();

            // Act
            var result = await repo.CreateReportComment(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(456, result.CommentId);
            Assert.Equal(123, dto.ReportId);
            _reportCommentDaoMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _reportCommentDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateReportComment_ShouldReturnNull_WhenReportIdIsInvalid()
        {
            // Arrange
            var dto = new ReportCommentDto
            {
                Content = "Invalid report",
                UserId = Guid.NewGuid()
            };
            var mappedReport = new Report { ReportId = 0 }; // invalid

            _mapperMock.Setup(m => m.Map<Report>(dto)).Returns(mappedReport);
            _reportDaoMock.Setup(d => d.AddAsync(It.IsAny<Report>()))
            .Returns(Task.CompletedTask);


            var repo = CreateRepository();

            // Act
            var result = await repo.CreateReportComment(dto);

            // Assert
            Assert.Null(result);
            _reportCommentDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            _reportCommentDaoMock.Verify(d => d.AddAsync(It.IsAny<ReportComment>()), Times.Never);
        }

        [Fact]
        public async Task CreateReportComment_ShouldReturnNull_WhenExceptionOccurs()
        {
            // Arrange
            var dto = new ReportCommentDto
            {
                Content = "Throw error",
                UserId = Guid.NewGuid()
            };

            _mapperMock.Setup(m => m.Map<Report>(dto)).Throws(new Exception("Mapping error"));

            var repo = CreateRepository();

            // Act
            var result = await repo.CreateReportComment(dto);

            // Assert
            Assert.Null(result);
            _reportCommentDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetAllReportComments_ShouldReturnPagedData_WhenDataExists()
        {
            // Arrange
            var data = new List<ReportComment>
        {
            new ReportComment { CommentId = 1, Report = new Report(), Comment = new Comment() },
            new ReportComment { CommentId = 2, Report = new Report(), Comment = new Comment() },
            new ReportComment { CommentId = 3, Report = new Report(), Comment = new Comment() }
        }.AsQueryable();

            _reportCommentDaoMock.Setup(d => d.GetAll()).Returns(data);

            var repo = CreateRepository();

            // Act
            var result = repo.GetAllReportComments(page: 1, pageSize: 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.PageSize);
        }

        [Fact]
        public void GetAllReportComments_ShouldReturnEmpty_WhenExceptionOccurs()
        {
            // Arrange
            _reportCommentDaoMock.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            var repo = CreateRepository();

            // Act
            var result = repo.GetAllReportComments(page: 1, pageSize: 2);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetReportComment_ShouldReturnComment_WhenFound()
        {
            // Arrange
            int id = 1;
            var expectedComment = new ReportComment { CommentId = 10 };

            _reportCommentDaoMock.Setup(d => d.BeginTransactionAsync())
                .Returns(Task.CompletedTask);
            _reportCommentDaoMock.Setup(d => d.GetByReportId(id))
                .Returns(expectedComment);
            _reportCommentDaoMock.Setup(d => d.CommitTransactionAsync())
                .Returns(Task.CompletedTask);

            var repo = CreateRepository();

            // Act
            var result = await repo.GetReportComment(id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedComment, result);

            _reportCommentDaoMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _reportCommentDaoMock.Verify(d => d.GetByReportId(id), Times.Once);
            _reportCommentDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _reportCommentDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task GetReportComment_ShouldRollbackAndReturnNull_WhenExceptionOccurs()
        {
            // Arrange
            int id = 1;

            _reportCommentDaoMock.Setup(d => d.BeginTransactionAsync())
                .Returns(Task.CompletedTask);
            _reportCommentDaoMock.Setup(d => d.GetByReportId(id))
                .Throws(new Exception("DB error"));
            _reportCommentDaoMock.Setup(d => d.RollbackTransactionAsync())
                .Returns(Task.CompletedTask);

            var repo = CreateRepository();

            // Act
            var result = await repo.GetReportComment(id);

            // Assert
            Assert.Null(result);

            _reportCommentDaoMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _reportCommentDaoMock.Verify(d => d.GetByReportId(id), Times.Once);
            _reportCommentDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            _reportCommentDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteReportsByCommentId_ShouldReturnFalse_WhenNoReportsFound()
        {
            // Arrange
            int commentId = 1;
            _reportDaoMock.Setup(d => d.BeginTransactionAsync())
                .Returns(Task.CompletedTask);
            _reportCommentDaoMock.Setup(d => d.GetByCommentId(commentId))
                .Returns((List<ReportComment>)null);
            _reportDaoMock.Setup(d => d.CommitTransactionAsync())
                .Returns(Task.CompletedTask);

            var repo = CreateRepository();

            // Act
            var result = await repo.DeleteReportsByCommentId(commentId);

            // Assert
            Assert.False(result);
            _reportDaoMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _reportDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _reportCommentDaoMock.Verify(d => d.DeleteByReportAndCommentId(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            _reportDaoMock.Verify(d => d.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteReportsByCommentId_ShouldReturnTrue_WhenReportsDeletedSuccessfully()
        {
            // Arrange
            int commentId = 1;
            var reportComments = new List<ReportComment>
    {
        new ReportComment { ReportId = 10, CommentId = commentId },
        new ReportComment { ReportId = 20, CommentId = commentId }
    };

            _reportDaoMock.Setup(d => d.BeginTransactionAsync())
                .Returns(Task.CompletedTask);
            _reportCommentDaoMock.Setup(d => d.GetByCommentId(commentId))
                .Returns(reportComments);
            _reportCommentDaoMock.Setup(d => d.DeleteByReportAndCommentId(It.IsAny<int>(), It.IsAny<int>()));
            _reportDaoMock.Setup(d => d.DeleteAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);
            _reportDaoMock.Setup(d => d.CommitTransactionAsync())
                .Returns(Task.CompletedTask);

            var repo = CreateRepository();

            // Act
            var result = await repo.DeleteReportsByCommentId(commentId);

            // Assert
            Assert.True(result);
            _reportDaoMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _reportCommentDaoMock.Verify(d => d.DeleteByReportAndCommentId(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(reportComments.Count));
            _reportDaoMock.Verify(d => d.DeleteAsync(It.IsAny<int>()), Times.Exactly(reportComments.Select(rc => rc.ReportId).Distinct().Count()));
            _reportDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteReportsByCommentId_ShouldReturnFalse_WhenExceptionOccurs()
        {
            // Arrange
            int commentId = 1;
            _reportDaoMock.Setup(d => d.BeginTransactionAsync())
                .Returns(Task.CompletedTask);
            _reportCommentDaoMock.Setup(d => d.GetByCommentId(commentId))
                .Throws(new Exception("DB error"));
            _reportDaoMock.Setup(d => d.RollbackTransactionAsync())
                .Returns(Task.CompletedTask);

            var repo = CreateRepository();

            // Act
            var result = await repo.DeleteReportsByCommentId(commentId);

            // Assert
            Assert.False(result);
            _reportDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            _reportDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Never);
        }

    }
}
