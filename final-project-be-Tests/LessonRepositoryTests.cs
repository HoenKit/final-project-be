using AutoMapper;
using Castle.Core.Logging;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Lesson;
using final_project_be_Domain.Models;
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
        private readonly IMapper _mapper;
        private readonly ILogger<LessonRepository> _logger;

        public LessonRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<LessonDto, Lesson>();
                cfg.CreateMap<UpdateLessonDto, Lesson>();
                cfg.CreateMap<Lesson, LessonResponseDto>();
            });

            _mapper = config.CreateMapper();
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<LessonRepository>();
        }

        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private LessonRepository CreateRepository(
            ApplicationDbContext context,
            out Mock<ICloudinaryService> cloudMock,
            out Mock<IBlobStorageService> blobMock)
        {
            cloudMock = new Mock<ICloudinaryService>();
            blobMock = new Mock<IBlobStorageService>();

            cloudMock.Setup(x => x.UploadVideoAndGetUrlAsync(It.IsAny<IFormFile>()))
                     .ReturnsAsync("https://video.example.com/fake.mp4");

            blobMock.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                    .Returns(Task.CompletedTask);

            blobMock.Setup(x => x.DeleteFileIfExistsAsync(It.IsAny<string>()))
                    .Returns(Task.CompletedTask);

            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<LessonRepository>();

            var lessonDAO = new NoTransactionLessonDAO(context);
            return new LessonRepository(lessonDAO, cloudMock.Object, _mapper, _logger, blobMock.Object);
        }

        [Fact]
        public async Task CreateLesson_ShouldAddLessonWithLinks()
        {
            var context = GetInMemoryDbContext();
            var repository = CreateRepository(context, out var cloudMock, out var blobMock);

            var videoFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("video")), 0, 5, "Video", "video.mp4");
            var docFile = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("doc")), 0, 5, "Document", "file.pdf");

            var dto = new LessonDto
            {
                ModuleId = 1,
                Title = "Lesson 1",
                Description = "Intro",
                Video = videoFile,
                Document = docFile
            };

            var result = await repository.CreateLesson(dto);

            Assert.NotNull(result);
            Assert.Equal("Lesson 1", result.Title);
            Assert.NotNull(result.VideoLink);
            Assert.NotNull(result.DocumentLink);
        }

        [Fact]
        public async Task UpdateLesson_ShouldModifyFieldsAndReplaceFiles()
        {
            var context = GetInMemoryDbContext();
            var repository = CreateRepository(context, out var cloudMock, out var blobMock);

            var existing = new Lesson
            {
                Title = "Old Lesson",
                ModuleId = 1,
                VideoLink = "https://video.old.com/vid.mp4",
                DocumentLink = "https://blob.old.com/doc.pdf"
            };

            context.Lessons.Add(existing);
            await context.SaveChangesAsync();

            var newVideo = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("new video")), 0, 10, "NewVideo", "new.mp4");
            var newDoc = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("new doc")), 0, 10, "NewDoc", "new.pdf");

            var dto = new UpdateLessonDto
            {
                LessonId = existing.LessonId,
                ModuleId = 1,
                Title = "Updated Lesson",
                Description = "Updated Description",
                Video = newVideo,
                Document = newDoc
            };

            var result = await repository.UpdateLesson(dto);

            Assert.NotNull(result);
            Assert.Equal("Updated Lesson", result.Title);
            Assert.StartsWith("https://video", result.VideoLink);
            Assert.StartsWith("https://finalprojectbestorage", result.DocumentLink);
        }

        [Fact]
        public async Task DeleteLesson_ShouldRemoveLesson()
        {
            var context = GetInMemoryDbContext();
            var repository = CreateRepository(context, out _, out _);

            var lesson = new Lesson { Title = "To Delete", ModuleId = 1 };
            context.Lessons.Add(lesson);
            await context.SaveChangesAsync();

            var result = await repository.DeleteLesson(lesson.LessonId);

            Assert.True(result);
            Assert.Null(context.Lessons.Find(lesson.LessonId));
        }

        [Fact]
        public async Task GetLesson_ShouldReturnLesson()
        {
            var context = GetInMemoryDbContext();
            var repository = CreateRepository(context, out _, out _);

            var lesson = new Lesson { Title = "Fetch Me", ModuleId = 1 };
            context.Lessons.Add(lesson);
            await context.SaveChangesAsync();

            var result = await repository.GetLesson(lesson.LessonId);

            Assert.NotNull(result);
            Assert.Equal("Fetch Me", result.Title);
        }

        [Fact]
        public async Task GetAllLessonByModuleId_ShouldReturnList()
        {
            var context = GetInMemoryDbContext();
            var repository = CreateRepository(context, out _, out _);

            context.Lessons.AddRange(
                new Lesson { Title = "L1", ModuleId = 999 },
                new Lesson { Title = "L2", ModuleId = 999 },
                new Lesson { Title = "L3", ModuleId = 888 }
            );
            await context.SaveChangesAsync();

            var result = await repository.GetAllLessonByModuleId(999);

            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal(999, r.ModuleId));
        }
    }

}
