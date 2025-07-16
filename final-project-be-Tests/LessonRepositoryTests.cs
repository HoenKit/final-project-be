using AutoMapper;
using Castle.Core.Logging;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Lesson;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using final_project_be_Tests.TestDAOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Tests
{
    public class LessonRepositoryTests
    {
        private readonly Mock<ILessonDAO> _lessonDaoMock = new();
        private readonly Mock<ICloudinaryService> _cloudinaryMock = new();
        private readonly Mock<IBlobStorageService> _blobMock = new();
        private readonly Mock<ILogger<LessonRepository>> _loggerMock = new();
        private readonly IMapper _mapper;

        public LessonRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<LessonDto, Lesson>();
                cfg.CreateMap<UpdateLessonDto, Lesson>();
                cfg.CreateMap<Lesson, LessonResponseDto>();
            });

            _mapper = config.CreateMapper();
        }

        private IFormFile GetMockFile(string name, string contentType = "text/plain")
        {
            var content = "dummy file content";
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            return new FormFile(stream, 0, stream.Length, "file", name)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        [Fact]
        public async Task CreateLesson_ShouldReturnLesson_WhenSuccess()
        {
            // Arrange
            var dto = new LessonDto
            {
                Title = "Test Lesson",
                Video = GetMockFile("video.mp4"),
                Document = GetMockFile("document.pdf")
            };

            _cloudinaryMock.Setup(x => x.UploadVideoAndGetUrlAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("https://cloudinary.com/video.mp4");
            _blobMock.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);
            _lessonDaoMock.Setup(x => x.AddAsync(It.IsAny<Lesson>())).Returns(Task.CompletedTask);
            _lessonDaoMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _lessonDaoMock.Setup(x => x.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var repo = new LessonRepository(_lessonDaoMock.Object, _cloudinaryMock.Object, _mapper, _loggerMock.Object, _blobMock.Object);

            // Act
            var result = await repo.CreateLesson(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Lesson", result.Title);
            Assert.NotNull(result.VideoLink);
            Assert.NotNull(result.DocumentLink);
        }

        [Fact]
        public async Task DeleteLesson_ShouldReturnTrue_WhenSuccess()
        {
            _lessonDaoMock.Setup(x => x.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            _lessonDaoMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _lessonDaoMock.Setup(x => x.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var repo = new LessonRepository(_lessonDaoMock.Object, _cloudinaryMock.Object, _mapper, _loggerMock.Object, _blobMock.Object);

            var result = await repo.DeleteLesson(1);

            Assert.True(result);
        }

        [Fact]
        public async Task GetAllLessonByModuleId_ShouldReturnList()
        {
            var lessons = new List<Lesson>
        {
            new Lesson { LessonId = 1, Title = "L1", ModuleId = 5 },
            new Lesson { LessonId = 2, Title = "L2", ModuleId = 5 }
        };

            _lessonDaoMock.Setup(x => x.GetAll()).Returns(lessons.AsQueryable());
            _lessonDaoMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _lessonDaoMock.Setup(x => x.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var repo = new LessonRepository(_lessonDaoMock.Object, _cloudinaryMock.Object, _mapper, _loggerMock.Object, _blobMock.Object);

            var result = await repo.GetAllLessonByModuleId(5);

            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal(5, lessons.First(x => x.LessonId == r.LessonId).ModuleId));
        }

        [Fact]
        public async Task GetLesson_ShouldReturnLesson_WhenFound()
        {
            var lesson = new Lesson { LessonId = 10, Title = "Test Lesson" };

            _lessonDaoMock.Setup(x => x.GetByIdAsync(10)).ReturnsAsync(lesson);
            _lessonDaoMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _lessonDaoMock.Setup(x => x.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var repo = new LessonRepository(_lessonDaoMock.Object, _cloudinaryMock.Object, _mapper, _loggerMock.Object, _blobMock.Object);

            var result = await repo.GetLesson(10);

            Assert.NotNull(result);
            Assert.Equal("Test Lesson", result.Title);
        }

        [Fact]
        public async Task UpdateLesson_ShouldUpdateAndReturnLesson()
        {
            var oldLesson = new Lesson
            {
                LessonId = 99,
                Title = "Old Title",
                VideoLink = "https://cloud/video_old.mp4",
                DocumentLink = "https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/old.pdf"
            };

            var dto = new UpdateLessonDto
            {
                LessonId = 99,
                Title = "New Title",
                Video = GetMockFile("video.mp4"),
                Document = GetMockFile("doc.pdf")
            };

            _lessonDaoMock.Setup(x => x.GetByIdAsync(99)).ReturnsAsync(oldLesson);
            _lessonDaoMock.Setup(x => x.UpdateAsync(It.IsAny<Lesson>())).Returns(Task.CompletedTask);
            _lessonDaoMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _lessonDaoMock.Setup(x => x.CommitTransactionAsync()).Returns(Task.CompletedTask);

            _cloudinaryMock.Setup(x => x.UploadVideoAndGetUrlAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync("https://cloud/video_new.mp4");

            _cloudinaryMock.Setup(x => x.DeleteVideoByUrlAsync(oldLesson.VideoLink))
                .ReturnsAsync(true);

            _blobMock.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                .Returns(Task.CompletedTask);

            _blobMock.Setup(x => x.DeleteFileIfExistsAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var repo = new LessonRepository(_lessonDaoMock.Object, _cloudinaryMock.Object, _mapper, _loggerMock.Object, _blobMock.Object);

            var result = await repo.UpdateLesson(dto);

            Assert.NotNull(result);
            Assert.Equal("New Title", result.Title);
            Assert.StartsWith("https://cloud/video_", result.VideoLink);
            Assert.StartsWith("https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/", result.DocumentLink);
        }
    }
}
