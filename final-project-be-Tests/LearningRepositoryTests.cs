using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.LearnDto;
using final_project_be_Domain.DTOs.Assignment;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using final_project_be_Application.Interface;

namespace final_project_be_Tests
{
    public class LearningRepositoryTests
    {
        private readonly Mock<IUserCourseDAO> _userCourseDaoMock = new();
        private readonly Mock<IUserLessonDAO> _userLessonDaoMock = new();
        private readonly Mock<IUserModuleDAO> _userModuleDaoMock = new();
        private readonly Mock<IUserAnswerDAO> _userAnswerDaoMock = new();
        private readonly Mock<IUserAssignmentDAO> _userAssignmentDaoMock = new();
        private readonly Mock<IAssignmentDAO> _assignmentDaoMock = new();
        private readonly Mock<IModuleDAO> _moduleDaoMock = new();
        private readonly Mock<ILessonDAO> _lessonDaoMock = new();
        private readonly Mock<IQuestionDAO> _questionDaoMock = new();
        private readonly Mock<ICaculator> _caculatorMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IBlobStorageService> _blobStorageServiceMock = new();
        private readonly Mock<ILogger<LearningRepository>> _loggerMock = new();

        private readonly LearningRepository _repository;

        public LearningRepositoryTests()
        {
            _repository = new LearningRepository(
                _userCourseDaoMock.Object,
                _userAssignmentDaoMock.Object,
                _userLessonDaoMock.Object,
                _blobStorageServiceMock.Object,
                _userAnswerDaoMock.Object,
                _lessonDaoMock.Object,
                _userModuleDaoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _moduleDaoMock.Object,
                _assignmentDaoMock.Object,
                _caculatorMock.Object,
                _questionDaoMock.Object
            );
        }

        [Fact]
        public async Task StartCourseAsync_ShouldAddUserCourseAndModules_WhenNotExists()
        {
            var userId = Guid.NewGuid();
            var courseId = 1;
            _userCourseDaoMock.Setup(d => d.UserCourseExists(userId, courseId)).ReturnsAsync(false);
            _moduleDaoMock.Setup(d => d.GetModulesByCourseId(courseId)).ReturnsAsync(new List<Module> { new Module { Title = "M1", Description = "D1", Lessons = new List<Lesson>(), UserModules = null } });
            _userModuleDaoMock.Setup(d => d.UserModuleExists(userId, It.IsAny<int>())).ReturnsAsync(false);
            _userModuleDaoMock.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask);

            await _repository.StartCourseAsync(userId, courseId);

            _userCourseDaoMock.Verify(d => d.AddUserCourseAsync(It.Is<UserCourse>(uc => uc.UserId == userId && uc.CourseId == courseId)), Times.Once);
            _userModuleDaoMock.Verify(d => d.AddUserModuleAsync(It.IsAny<UserModule>()), Times.Once);
            _userModuleDaoMock.Verify(d => d.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CompleteLessonAsync_ShouldThrow_WhenLessonNotFound()
        {
            var userId = Guid.NewGuid();
            var lessonId = 1;
            _userLessonDaoMock.Setup(d => d.GetUserLessonbyuserandlessonAsync(userId, lessonId)).ReturnsAsync((UserLesson)null);
            _lessonDaoMock.Setup(d => d.GetLessonByIdAsync(lessonId)).ReturnsAsync((Lesson)null);

            await Assert.ThrowsAsync<Exception>(() => _repository.CompleteLessonAsync(userId, lessonId, 90));
        }

        [Fact]
        public async Task CompleteLessonAsync_ShouldThrow_WhenQuizScoreIsNull()
        {
            var userId = Guid.NewGuid();
            var lessonId = 1;
            _userLessonDaoMock.Setup(d => d.GetUserLessonbyuserandlessonAsync(userId, lessonId)).ReturnsAsync((UserLesson)null);
            _lessonDaoMock.Setup(d => d.GetLessonByIdAsync(lessonId)).ReturnsAsync(new Lesson());
            _lessonDaoMock.Setup(d => d.HasQuestionAsync(lessonId)).ReturnsAsync(true);

            await Assert.ThrowsAsync<ArgumentException>(() => _repository.CompleteLessonAsync(userId, lessonId, null));
        }

        [Fact]
        public async Task CompleteLessonAsync_ShouldReturnUserLesson_WhenDocsOrVideo()
        {
            var userId = Guid.NewGuid();
            var lessonId = 1;
            _userLessonDaoMock.Setup(d => d.GetUserLessonbyuserandlessonAsync(userId, lessonId)).ReturnsAsync((UserLesson)null);
            _lessonDaoMock.Setup(d => d.GetLessonByIdAsync(lessonId)).ReturnsAsync(new Lesson { DocumentLink = "doc.pdf" });
            _lessonDaoMock.Setup(d => d.HasQuestionAsync(lessonId)).ReturnsAsync(false);
            _lessonDaoMock.Setup(d => d.HasAssignmentAsync(lessonId)).ReturnsAsync(false);
            _userLessonDaoMock.Setup(d => d.AddUserLessonAsync(It.IsAny<UserLesson>())).Returns(Task.CompletedTask);
            _userLessonDaoMock.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CompleteLessonAsync(userId, lessonId, null);

            Assert.NotNull(result);
            Assert.True(result.IsPassed);
            Assert.Equal(100, result.Mark);
        }

        [Fact]
        public async Task CreateUserAssignmentAsync_ShouldCreateAndReturn_WhenNotExists()
        {
            var dto = new CreateUserAssignmentDto { UserId = Guid.NewGuid(), AssignmentId = 1 };
            _userAssignmentDaoMock.Setup(d => d.GetUserAssignmentAsync(dto.UserId, dto.AssignmentId)).ReturnsAsync((UserAssignment)null);
            _userAssignmentDaoMock.Setup(d => d.CreateUserAssignmentAsync(dto)).ReturnsAsync(new UserAssignment());
            _assignmentDaoMock.Setup(d => d.GetByIdAsync(dto.AssignmentId)).ReturnsAsync(new Assignment { LessonId = 2 });
            _userLessonDaoMock.Setup(d => d.GetUserLessonbyuserandlessonAsync(dto.UserId, 2)).ReturnsAsync((UserLesson)null);
            _userLessonDaoMock.Setup(d => d.AddUserLessonAsync(It.IsAny<UserLesson>())).Returns(Task.CompletedTask);

            var result = await _repository.CreateUserAssignmentAsync(dto);

            Assert.NotNull(result);
            _userLessonDaoMock.Verify(d => d.AddUserLessonAsync(It.IsAny<UserLesson>()), Times.Once);
        }

        [Fact]
        public async Task SubmitQuizAsync_ShouldThrow_WhenLessonNotFound()
        {
            var dto = new SubmitQuizDto { UserId = Guid.NewGuid(), LessonId = 1, AnswerIds = new List<int>() };
            _lessonDaoMock.Setup(d => d.GetLessonByIdAsync(dto.LessonId)).ReturnsAsync((Lesson)null);

            await Assert.ThrowsAsync<Exception>(() => _repository.SubmitQuizAsync(dto));
        }

        [Fact]
        public async Task SubmitQuizAsync_ShouldReturnScore_WhenSuccess()
        {
            var dto = new SubmitQuizDto { UserId = Guid.NewGuid(), LessonId = 1, AnswerIds = new List<int> { 1, 2 } };
            var userLesson = new UserLesson { UserLessonId = 10, UserId = dto.UserId, LessonId = dto.LessonId };
            _lessonDaoMock.Setup(d => d.GetLessonByIdAsync(dto.LessonId)).ReturnsAsync(new Lesson());
            _lessonDaoMock.Setup(d => d.IsQuizLessonAsync(dto.LessonId)).ReturnsAsync(true);
            _lessonDaoMock.Setup(d => d.GetUserLessonAsync(dto.UserId, dto.LessonId)).ReturnsAsync(userLesson);
            _userAnswerDaoMock.Setup(d => d.DeleteUserAnswersByUserLessonIdAsync(userLesson.UserLessonId)).Returns(Task.CompletedTask);
            _lessonDaoMock.Setup(d => d.AddUserAnswersAsync(It.IsAny<List<UserAnswer>>())).Returns(Task.CompletedTask);
            _lessonDaoMock.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask);
            _caculatorMock.Setup(d => d.CalculateQuizScore(dto.UserId, dto.LessonId)).ReturnsAsync((95f, true));

            var result = await _repository.SubmitQuizAsync(dto);

            Assert.Equal(95f, result);
        }

        [Fact]
        public async Task UploadCertificateAndSaveLinkAsync_ShouldReturnNull_WhenUserCourseNotFound()
        {
            var dto = new CertificateUploadDto { UserId = Guid.NewGuid(), CourseId = 1, CertificateFile = Mock.Of<IFormFile>() };
            _userCourseDaoMock.Setup(d => d.GetCompletedUserCourseAsync(dto.UserId, dto.CourseId)).ReturnsAsync((UserCourse)null);

            var result = await _repository.UploadCertificateAndSaveLinkAsync(dto);

            Assert.Null(result);
        }

        [Fact]
        public async Task UploadCertificateAndSaveLinkAsync_ShouldReturnCertificateLink_WhenSuccess()
        {
            // Arrange
            var dto = new CertificateUploadDto
            {
                UserId = Guid.NewGuid(),
                CourseId = 1,
                CertificateFile = Mock.Of<IFormFile>(f =>
                    f.FileName == "cert.pdf" &&
                    f.OpenReadStream() == new MemoryStream(new byte[1]))
            };

            _userCourseDaoMock
                .Setup(d => d.GetCompletedUserCourseAsync(dto.UserId, dto.CourseId))
                .ReturnsAsync(new UserCourse());

            _blobStorageServiceMock
                .Setup(s => s.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);

            _userCourseDaoMock
                .Setup(d => d.UpdateCertificateLinkAsync(dto.UserId, dto.CourseId, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _repository.UploadCertificateAndSaveLinkAsync(dto);

            // Assert
            Assert.NotNull(result); // ✅ kết quả không null
            Assert.Contains("https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/", result); // ✅ đúng prefix

            _blobStorageServiceMock.Verify(s => s.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once);
            _userCourseDaoMock.Verify(d => d.UpdateCertificateLinkAsync(dto.UserId, dto.CourseId, It.IsAny<string>()), Times.Once);
        }
    }
}