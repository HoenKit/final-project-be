using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.Data;
using final_project_be_Tests.TestDAOs;
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
    public class CourseRepositoryTests
    {
        private readonly IMapper _mapper;

        public CourseRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CourseDto, Courses>();
                cfg.CreateMap<UpdateCourseDto, Courses>();
                cfg.CreateMap<Courses, CourseResponseDto>();
            });
            _mapper = config.CreateMapper();
        }

        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        private CourseRepository CreateRepository(ApplicationDbContext context, out Mock<IBlobStorageService> blobMock)
        {
            blobMock = new Mock<IBlobStorageService>();
            blobMock.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                    .Returns(Task.CompletedTask);
            blobMock.Setup(x => x.DeleteFileIfExistsAsync(It.IsAny<string>()))
                    .Returns(Task.CompletedTask);

            var courseDao = new NoTransactionCourseDAO(context);
            var reviewDao = new NoTransactionReviewDAO(context);
            var userCourseDao = new NoTransactionUserCourseDAO(context);

            var lessonDao = new NoTransactionLessonDAO(context);
            var moduleDao = new NoTransactionModuleDAO(context);
            var userModuleDao = new NoTransactionUserModuleDAO(context);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            var courseLogger = loggerFactory.CreateLogger<CourseRepository>();
            var calculatorLogger = loggerFactory.CreateLogger<Caculator>();

            var calculator = new Caculator(lessonDao, moduleDao, userCourseDao, userModuleDao);

            return new CourseRepository(
                courseDao,
                calculator,
                userCourseDao,
                reviewDao,
                _mapper,
                courseLogger,
                blobMock.Object
            );
        }

        [Fact]
        public async Task CreateCourse_ShouldAddCourse()
        {
            var context = GetInMemoryDbContext();
            var repo = CreateRepository(context, out var blobMock);

            var image = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("img")), 0, 3, "Image", "img.jpg");

            var dto = new CourseDto
            {
                CourseName = "Test Course",
                CategoryId = 1,
                MentorId = 1,
                CourseContent = "Content",
                Cost = 100,
                SkillLearn = "Skill",
                CoursesImage = image
            };

            var result = await repo.CreateCourse(dto);

            Assert.NotNull(result);
            Assert.Equal("Test Course", result.CourseName);
            Assert.StartsWith("https://", result.CoursesImage);
        }

        [Fact]
        public async Task ToggleIsDeleted_ShouldSwitchFlag()
        {
            var context = GetInMemoryDbContext();
            var course = new Courses { CourseName = "Course A", IsDeleted = false };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var repo = CreateRepository(context, out var _);
            var result = await repo.ToggleIsDeleted(course.CourseId);

            Assert.True(result.IsDeleted);
        }

        [Fact]
        public async Task ToggleStatus_ShouldUpdateStatus()
        {
            var context = GetInMemoryDbContext();
            var course = new Courses { CourseName = "Course B", Status = "Pending" };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var repo = CreateRepository(context, out var _);
            var result = await repo.ToggleStatus(course.CourseId, "Approved");

            Assert.Equal("Approved", result.Status);
        }

        [Fact]
        public async Task UpdateCourse_ShouldUpdateFieldsAndReplaceImage()
        {
            var context = GetInMemoryDbContext();
            var course = new Courses { CourseName = "Old", Status = "Approved", CoursesImage = "https://old.img.com/file.jpg" };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var repo = CreateRepository(context, out var blobMock);

            var newImage = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("newimg")), 0, 6, "NewImage", "new.jpg");

            var dto = new UpdateCourseDto
            {
                CourseId = course.CourseId,
                CourseName = "New",
                Cost = 150,
                CoursesImage = newImage
            };

            var result = await repo.UpdateCourse(dto);

            Assert.NotNull(result);
            Assert.Equal("New", result.CourseName);
            Assert.StartsWith("https://", result.CoursesImage);
        }

        [Fact]
        public void GetAllCourses_ShouldReturnFilteredPaginatedCourses()
        {
            var context = GetInMemoryDbContext();
            context.Courses.AddRange(
                new Courses { CourseName = "A", Cost = 100, Status = "Approved", CreateAt = DateTime.Now },
                new Courses { CourseName = "B", Cost = 200, Status = "Approved", CreateAt = DateTime.Now },
                new Courses { CourseName = "C", Cost = 300, Status = "Pending", CreateAt = DateTime.Now }
            );
            context.SaveChanges();

            var repo = CreateRepository(context, out var _);

            var result = repo.GetAllCourses(1, 10, null, null, null, null, null, null, null, null, null, null, null, new List<StatusEnum> { StatusEnum.Approved });

            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task GetCourse_ShouldReturnCorrectCourse()
        {
            var context = GetInMemoryDbContext();
            var course = new Courses { CourseName = "Course 1", CategoryId = 1, MentorId = 1 };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var repository = CreateRepository(context, out var _);

            var result = await repository.GetCourse(course.CourseId);

            Assert.NotNull(result);
            Assert.Equal("Course 1", result.CourseName);
        }
    }

}
