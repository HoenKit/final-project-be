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
    public class ReportCourseRepositoryTests
    {
        private readonly Mock<IReportCourseDAO> _reportCourseDaoMock;
        private readonly Mock<IReportDAO> _reportDaoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ReportCourseRepository>> _loggerMock;

        private readonly ReportCourseRepository _repository;

        public ReportCourseRepositoryTests()
        {
            _reportCourseDaoMock = new Mock<IReportCourseDAO>();
            _reportDaoMock = new Mock<IReportDAO>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ReportCourseRepository>>();

            _repository = new ReportCourseRepository(
                _reportCourseDaoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _reportDaoMock.Object
            );
        }

        [Fact]
        public async Task CreateReportCourse_ShouldReturnReportCourse_WhenSuccess()
        {
            // Arrange
            var dto = new ReportCourseDto
            {
                CourseId = 1,
                UserId = Guid.NewGuid(),
                Content = "Test Content"
            };

            var mappedReport = new Report { ReportId = 123 };
            var mappedReportCourse = new ReportCourse { ReportId = 123, CourseId = 1 };

            _mapperMock.Setup(m => m.Map<Report>(dto)).Returns(mappedReport);
            _mapperMock.Setup(m => m.Map<ReportCourse>(dto)).Returns(mappedReportCourse);

            _reportDaoMock.Setup(d => d.AddAsync(mappedReport)).Returns(Task.CompletedTask);
            _reportCourseDaoMock.Setup(d => d.AddAsync(mappedReportCourse)).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateReportCourse(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(123, dto.ReportId);
            Assert.Equal(123, result.ReportId);

            _reportCourseDaoMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _reportCourseDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _reportCourseDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateReportCourse_ShouldRollback_WhenReportCreationFails()
        {
            // Arrange
            var dto = new ReportCourseDto { Content = "Invalid Report" };
            Report nullReport = null;

            _mapperMock.Setup(m => m.Map<Report>(dto)).Returns(nullReport);

            // Act
            var result = await _repository.CreateReportCourse(dto);

            // Assert
            Assert.Null(result);
            _reportCourseDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            _reportCourseDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateReportCourse_ShouldRollback_WhenExceptionThrown()
        {
            // Arrange
            var dto = new ReportCourseDto { Content = "Exception Case" };
            var mappedReport = new Report { ReportId = 999 };

            _mapperMock.Setup(m => m.Map<Report>(dto)).Returns(mappedReport);
            _reportDaoMock.Setup(d => d.AddAsync(It.IsAny<Report>()))
                .ThrowsAsync(new Exception("DB Error"));

            // Act
            var result = await _repository.CreateReportCourse(dto);

            // Assert
            Assert.Null(result);
            _reportCourseDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetAllReportCourses_ReturnsPagedResult()
        {
            // Arrange
            var mockDao = new Mock<IReportCourseDAO>();
            var mockLogger = new Mock<ILogger<ReportCourseRepository>>();
            var mockReportDao = new Mock<IReportDAO>();
            var mockMapper = new Mock<IMapper>();

            // Giả dữ liệu EF queryable
            var data = new List<ReportCourse>
        {
            new ReportCourse { ReportId = 1, CourseId = 101 },
            new ReportCourse { ReportId = 2, CourseId = 102 },
            new ReportCourse { ReportId = 3, CourseId = 103 }
        }.AsQueryable();

            mockDao.Setup(d => d.GetAll()).Returns(data);

            var repo = new ReportCourseRepository(mockDao.Object, mockMapper.Object, mockLogger.Object, mockReportDao.Object);

            // Act
            var result = repo.GetAllReportCourses(page: 1, pageSize: 2);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount); 
            Assert.Equal(2, result.Items.Count()); 
            Assert.Equal(1, result.CurrentPage);       
            Assert.Equal(2, result.PageSize);
        }

        [Fact]
        public void GetAllReportCourses_WhenException_ReturnsEmptyResult()
        {
            // Arrange
            var mockDao = new Mock<IReportCourseDAO>();
            var mockLogger = new Mock<ILogger<ReportCourseRepository>>();
            var mockReportDao = new Mock<IReportDAO>();
            var mockMapper = new Mock<IMapper>();

            mockDao.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            var repo = new ReportCourseRepository(mockDao.Object, mockMapper.Object, mockLogger.Object, mockReportDao.Object);

            // Act
            var result = repo.GetAllReportCourses(page: 1, pageSize: 2);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetReportCourse_Success_ReturnsReportCourse()
        {
            // Arrange
            var mockDao = new Mock<IReportCourseDAO>();
            var mockLogger = new Mock<ILogger<ReportCourseRepository>>();
            var mockReportDao = new Mock<IReportDAO>();
            var mockMapper = new Mock<IMapper>();

            var expected = new ReportCourse { ReportId = 1, CourseId = 101 };

            mockDao.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.GetByReportId(1)).Returns(expected);
            mockDao.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var repo = new ReportCourseRepository(mockDao.Object, mockMapper.Object, mockLogger.Object, mockReportDao.Object);

            // Act
            var result = await repo.GetReportCourse(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected.ReportId, result.ReportId);
            mockDao.Verify(d => d.BeginTransactionAsync(), Times.Once);
            mockDao.Verify(d => d.CommitTransactionAsync(), Times.Once);
            mockDao.Verify(d => d.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task GetReportCourse_Exception_ReturnsNull()
        {
            // Arrange
            var mockDao = new Mock<IReportCourseDAO>();
            var mockLogger = new Mock<ILogger<ReportCourseRepository>>();
            var mockReportDao = new Mock<IReportDAO>();
            var mockMapper = new Mock<IMapper>();

            mockDao.Setup(d => d.BeginTransactionAsync()).ThrowsAsync(new Exception("DB error"));
            mockDao.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var repo = new ReportCourseRepository(mockDao.Object, mockMapper.Object, mockLogger.Object, mockReportDao.Object);

            // Act
            var result = await repo.GetReportCourse(1);

            // Assert
            Assert.Null(result);
            mockDao.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            mockDao.Verify(d => d.CommitTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteReportsByCourseId_HasData_ReturnsTrue()
        {
            // Arrange
            var mockReportDao = new Mock<IReportDAO>();
            var mockReportCourseDao = new Mock<IReportCourseDAO>();
            var mockLogger = new Mock<ILogger<ReportCourseRepository>>();
            var mockMapper = new Mock<IMapper>();

            var courseId = 101;
            var reportCourses = new List<ReportCourse>
        {
            new ReportCourse { ReportId = 1, CourseId = courseId },
            new ReportCourse { ReportId = 2, CourseId = courseId }
        };

            mockReportDao.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockReportDao.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);
            mockReportCourseDao.Setup(d => d.GetByCourseId(courseId)).Returns(reportCourses);
            mockReportCourseDao.Setup(d => d.DeleteByReportAndCourseId(It.IsAny<int>(), It.IsAny<int>()));
            mockReportDao.Setup(d => d.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);

            var repo = new ReportCourseRepository(mockReportCourseDao.Object, mockMapper.Object, mockLogger.Object, mockReportDao.Object);

            // Act
            var result = await repo.DeleteReportsByCourseId(courseId);

            // Assert
            Assert.True(result);
            mockReportDao.Verify(d => d.CommitTransactionAsync(), Times.Once);
            mockReportDao.Verify(d => d.RollbackTransactionAsync(), Times.Never);
            mockReportCourseDao.Verify(d => d.DeleteByReportAndCourseId(It.IsAny<int>(), courseId), Times.Exactly(reportCourses.Count));
            mockReportDao.Verify(d => d.DeleteAsync(It.IsAny<int>()), Times.Exactly(reportCourses.Count));
        }

        [Fact]
        public async Task DeleteReportsByCourseId_NoData_ReturnsFalse()
        {
            // Arrange
            var mockReportDao = new Mock<IReportDAO>();
            var mockReportCourseDao = new Mock<IReportCourseDAO>();
            var mockLogger = new Mock<ILogger<ReportCourseRepository>>();
            var mockMapper = new Mock<IMapper>();

            var courseId = 101;

            mockReportDao.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockReportDao.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);
            mockReportCourseDao.Setup(d => d.GetByCourseId(courseId)).Returns(new List<ReportCourse>());

            var repo = new ReportCourseRepository(mockReportCourseDao.Object, mockMapper.Object, mockLogger.Object, mockReportDao.Object);

            // Act
            var result = await repo.DeleteReportsByCourseId(courseId);

            // Assert
            Assert.False(result);
            mockReportDao.Verify(d => d.CommitTransactionAsync(), Times.Once);
            mockReportDao.Verify(d => d.RollbackTransactionAsync(), Times.Never);
            mockReportCourseDao.Verify(d => d.DeleteByReportAndCourseId(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
            mockReportDao.Verify(d => d.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task DeleteReportsByCourseId_Exception_ReturnsFalse()
        {
            // Arrange
            var mockReportDao = new Mock<IReportDAO>();
            var mockReportCourseDao = new Mock<IReportCourseDAO>();
            var mockLogger = new Mock<ILogger<ReportCourseRepository>>();
            var mockMapper = new Mock<IMapper>();

            var courseId = 101;

            mockReportDao.Setup(d => d.BeginTransactionAsync()).ThrowsAsync(new Exception("DB Error"));
            mockReportDao.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var repo = new ReportCourseRepository(mockReportCourseDao.Object, mockMapper.Object, mockLogger.Object, mockReportDao.Object);

            // Act
            var result = await repo.DeleteReportsByCourseId(courseId);

            // Assert
            Assert.False(result);
            mockReportDao.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            mockReportDao.Verify(d => d.CommitTransactionAsync(), Times.Never);
        }
    }
}
