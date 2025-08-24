using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Service.GoogleMeetService;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Assignment;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace final_project_be_Tests
{
    public class AssignmentRepositoryTests
    {
        private readonly Mock<IAssignmentDAO> _assignmentDaoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IGoogleMeetService> _googleMeetServiceMock;
        private readonly Mock<ILogger<AssignmentRepository>> _loggerMock;
        private readonly Mock<ILessonDAO> _lessonDaoMock;
        private readonly IOptions<ClientSettings> _clientSettings;
        private readonly AssignmentRepository _repository;

        public AssignmentRepositoryTests()
        {
            _assignmentDaoMock = new Mock<IAssignmentDAO>();
            _mapperMock = new Mock<IMapper>();
            _lessonDaoMock = new Mock<ILessonDAO>();
            _googleMeetServiceMock = new Mock<IGoogleMeetService>();
            _loggerMock = new Mock<ILogger<AssignmentRepository>>();
            _clientSettings = Options.Create(new ClientSettings { BaseUrl = "http://localhost" });

            _repository = new AssignmentRepository(
                _assignmentDaoMock.Object,
                _mapperMock.Object,
                _googleMeetServiceMock.Object,
                _loggerMock.Object,
                _clientSettings,
                _lessonDaoMock.Object
            );
        }
        [Fact]
        public async Task CreateAssignment_ShouldCreateAssignmentWithMeetLink_WhenNoLinkProvided()
        {
            // Arrange
            var dto = new AssignmentDto { LessonId = 1, Content = "Test", MeetLink = null };
            var assignment = new Assignment { LessonId = 1, Content = "Test", MeetLink = null };

            _mapperMock
                .Setup(m => m.Map<Assignment>(dto))
                .Returns(assignment);

            _lessonDaoMock
                .Setup(m => m.GetCourseIdByLessonIdAsync(dto.LessonId))
                .ReturnsAsync(10); // course tồn tại

            _assignmentDaoMock
                .Setup(m => m.HasAssignmentByCourseIdAsync(10))
                .ReturnsAsync(false); // chưa có assignment

            _googleMeetServiceMock
                .Setup(m => m.CreateGoogleMeetLinkAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync("http://meet.link");

            _assignmentDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _assignmentDaoMock.Setup(m => m.AddAsync(It.IsAny<Assignment>())).Returns(Task.CompletedTask);
            _assignmentDaoMock.Setup(m => m.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateAssignment(dto);

            // Assert
            Assert.NotNull(result.assignment);
            Assert.Equal("http://meet.link", result.assignment.MeetLink);
            Assert.Equal("Assignment created successfully", result.message);

            _assignmentDaoMock.Verify(m => m.AddAsync(It.IsAny<Assignment>()), Times.Once);
            _assignmentDaoMock.Verify(m => m.CommitTransactionAsync(), Times.Once);
            _assignmentDaoMock.Verify(m => m.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAssignment_ShouldReturnNullAssignmentAndRollback_WhenExceptionThrown()
        {
            // Arrange
            var dto = new AssignmentDto { LessonId = 1, Content = "Test" };
            _mapperMock.Setup(m => m.Map<Assignment>(dto)).Throws(new Exception("Mapping failed"));
            _assignmentDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _assignmentDaoMock.Setup(m => m.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateAssignment(dto);

            // Assert
            Assert.Null(result.assignment); // ✅ kiểm tra assignment = null
            Assert.Equal("Unexpected error when creating assignment", result.message); // ✅ kiểm tra message
            _assignmentDaoMock.Verify(m => m.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAssignment_ShouldReturnTrue_WhenDeleteSucceeds()
        {
            // Arrange
            _assignmentDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _assignmentDaoMock.Setup(m => m.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            _assignmentDaoMock.Setup(m => m.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.DeleteAssignment(1);

            // Assert
            Assert.True(result);
            _assignmentDaoMock.Verify(m => m.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAssignment_ShouldReturnFalseAndRollback_WhenExceptionThrown()
        {
            // Arrange
            _assignmentDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _assignmentDaoMock.Setup(m => m.DeleteAsync(It.IsAny<int>())).Throws(new Exception("Delete failed"));
            _assignmentDaoMock.Setup(m => m.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.DeleteAssignment(1);

            // Assert
            Assert.False(result);
            _assignmentDaoMock.Verify(m => m.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAssignmentsBycreatorAsync_ShouldReturnMappedDtos()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var assignments = new List<Assignment>
            {
                new Assignment { AssignmentId = 1, LessonId = 2, Content = "A", MeetLink = "link", Lesson = new Lesson { Title = "T" } }
            };
            _assignmentDaoMock.Setup(m => m.GetAssignmentsByUserIdAsync(userId)).ReturnsAsync(assignments);

            // Act
            var result = await _repository.GetAssignmentsBycreatorAsync(userId);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0].AssignmentId);
            Assert.Equal("T", result[0].Title);
        }

        [Fact]
        public async Task GetAllAssignmentByLessonId_ShouldReturnDtos_WhenSuccess()
        {
            // Arrange
            var lessonId = 1;
            var assignments = new List<Assignment>
            {
                new Assignment { AssignmentId = 1, LessonId = lessonId, Content = "A", MeetLink = "link", CreateAt = DateTime.Now }
            };
            var dtos = new List<AssignmentResponseDto>
            {
                new AssignmentResponseDto { AssignmentId = 1, LessonId = lessonId, Content = "A", MeetLink = "link", CreateAt = DateTime.Now }
            };
            _assignmentDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _assignmentDaoMock.Setup(m => m.GetAll()).Returns(assignments.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<AssignmentResponseDto>>(It.IsAny<List<Assignment>>())).Returns(dtos);
            _assignmentDaoMock.Setup(m => m.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.GetAllAssignmentByLessonId(lessonId);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().AssignmentId);
        }

        [Fact]
        public async Task GetAllAssignmentByLessonId_ShouldReturnEmptyList_WhenExceptionThrown()
        {
            // Arrange
            _assignmentDaoMock.Setup(m => m.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _assignmentDaoMock.Setup(m => m.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.GetAllAssignmentByLessonId(1);

            // Assert
            Assert.Empty(result);
            _assignmentDaoMock.Verify(m => m.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAssignment_ShouldReturnAssignment_WhenFound()
        {
            // Arrange
            var assignment = new Assignment { AssignmentId = 1 };
            _assignmentDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _assignmentDaoMock.Setup(m => m.GetByIdAsync(1)).ReturnsAsync(assignment);
            _assignmentDaoMock.Setup(m => m.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.GetAssignment(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.AssignmentId);
        }

        [Fact]
        public async Task GetAssignment_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            // Arrange
            _assignmentDaoMock.Setup(m => m.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _assignmentDaoMock.Setup(m => m.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.GetAssignment(1);

            // Assert
            Assert.Null(result);
            _assignmentDaoMock.Verify(m => m.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAssignment_ShouldUpdateAndReturnAssignment_WhenFound()
        {
            // Arrange
            var dto = new UpdateAssignmentDto { AssignmentId = 1, LessonId = 2, Content = "Updated" };
            var assignment = new Assignment { AssignmentId = 1, LessonId = 2, Content = "Old" };
            _assignmentDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _assignmentDaoMock.Setup(m => m.GetByIdAsync(dto.AssignmentId)).ReturnsAsync(assignment);
            _mapperMock.Setup(m => m.Map(dto, assignment)).Verifiable();
            _assignmentDaoMock.Setup(m => m.UpdateAsync(assignment)).Returns(Task.CompletedTask);
            _assignmentDaoMock.Setup(m => m.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.UpdateAssignment(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.AssignmentId);
            _assignmentDaoMock.Verify(m => m.UpdateAsync(assignment), Times.Once);
        }

        [Fact]
        public async Task UpdateAssignment_ShouldReturnNullAndRollback_WhenAssignmentNotFound()
        {
            // Arrange
            var dto = new UpdateAssignmentDto { AssignmentId = 1 };
            _assignmentDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _assignmentDaoMock.Setup(m => m.GetByIdAsync(dto.AssignmentId)).ReturnsAsync((Assignment)null);
            _assignmentDaoMock.Setup(m => m.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.UpdateAssignment(dto);

            // Assert
            Assert.Null(result);
            _assignmentDaoMock.Verify(m => m.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAssignment_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            // Arrange
            var dto = new UpdateAssignmentDto { AssignmentId = 1 };
            _assignmentDaoMock.Setup(m => m.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _assignmentDaoMock.Setup(m => m.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.UpdateAssignment(dto);

            // Assert
            Assert.Null(result);
            _assignmentDaoMock.Verify(m => m.RollbackTransactionAsync(), Times.Once);
        }
    }
}