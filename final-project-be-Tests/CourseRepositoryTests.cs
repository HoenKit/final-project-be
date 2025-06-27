using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.Data;
using final_project_be_Tests.TestDAOs;
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
        private readonly ILogger<CourseRepository> _logger;
        private readonly Mock<IBlobStorageService> _blobMock;

        public CourseRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CourseDto, Courses>();
                cfg.CreateMap<Courses, CourseResponseDto>();
            });
            _mapper = config.CreateMapper();
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CourseRepository>();
            _blobMock = new Mock<IBlobStorageService>();
            _blobMock.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.CompletedTask);
            _blobMock.Setup(x => x.DeleteFileIfExistsAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        }

        private ApplicationDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private CourseRepository CreateRepository(ApplicationDbContext context)
        {
            var courseDao = new NoTransactionCourseDAO(context);
            var reviewDao = new NoTransactionReviewDAO(context);
            return new CourseRepository(courseDao, reviewDao, _mapper, _logger, _blobMock.Object);
        }

        [Fact]
        public async Task CreateCourse_ShouldAddCourse()
        {
            var context = CreateContext();
            var repository = CreateRepository(context);

            var courseDto = new CourseDto
            {
                CourseName = "Test Course",
                CategoryId = 1,
                MentorId = 1,
                CourseContent = "Content",
                Cost = 100,
                SkillLearn = "Skills"
            };

            var result = await repository.CreateCourse(courseDto);

            Assert.NotNull(result);
            Assert.Equal("Test Course", result.CourseName);
        }

        [Fact]
        public async Task GetCourse_ShouldReturnCorrectCourse()
        {
            var context = CreateContext();
            var course = new Courses { CourseName = "Course 1", CategoryId = 1, MentorId = 1 };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var repository = CreateRepository(context);

            var result = await repository.GetCourse(course.CourseId);

            Assert.NotNull(result);
            Assert.Equal("Course 1", result.CourseName);
        }

        [Fact]
        public async Task ToggleIsDeleted_ShouldSwitchFlag()
        {
            var context = CreateContext();
            var course = new Courses { CourseName = "Course 2", IsDeleted = false };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var repository = CreateRepository(context);
            var result = await repository.ToggleIsDeleted(course.CourseId);

            Assert.True(result.IsDeleted);
        }

        [Fact]
        public async Task ToggleStatus_ShouldUpdateStatus()
        {
            var context = CreateContext();
            var course = new Courses { CourseName = "Course 3", Status = "Pending" };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var repository = CreateRepository(context);
            var result = await repository.ToggleStatus(course.CourseId, "Approved");

            Assert.Equal("Approved", result.Status);
        }

        [Fact]
        public void GetAllCourses_ShouldReturnFilteredPaginatedCourses()
        {
            // Arrange
            var context = CreateContext();

            // Add test data
            context.Courses.AddRange(
                new Courses { CourseName = "Course A", Cost = 100, Status = StatusEnum.Approved.ToString(), CreateAt = DateTime.Now },
                new Courses { CourseName = "Course B", Cost = 200, Status = StatusEnum.Approved.ToString(), CreateAt = DateTime.Now },
                new Courses { CourseName = "Course C", Cost = 300, Status = StatusEnum.Pending.ToString(), CreateAt = DateTime.Now }
            );
            context.SaveChanges();

            // DAO + Logger + Repo
            var dao = new NoTransactionCourseDAO(context);
            var reviewDao = new NoTransactionReviewDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CourseRepository>();
            var blobMock = new Mock<IBlobStorageService>();

            var repo = new CourseRepository(dao, reviewDao, _mapper, logger, blobMock.Object);

            var statuses = new List<StatusEnum> { StatusEnum.Approved };

            // Act
            var result = repo.GetAllCourses(
                page: 1,
                pageSize: 10,
                CategoryId: null,
                title: null,
                userId: null,
                sortOption: null,
                mentorId: null,
                Language: null,
                Level: null,
                MinCost: null,
                MaxCost: null,
                MinRate: null,
                MaxRate: null,
                statuses: statuses
            );

            // Assert
            logger.LogInformation($"TotalCount: {result.TotalCount}, ItemCount: {result.Items.Count()}");

            foreach (var item in result.Items)
            {
                logger.LogInformation($"Course: {item.CourseName}, Status: {item.Status}, Cost: {item.Cost}");
            }

            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task UpdateCourse_ShouldModifyFields()
        {
            var context = CreateContext();
            var existing = new Courses { CourseName = "Old", Cost = 100, Status = "Approved" };
            context.Courses.Add(existing);
            await context.SaveChangesAsync();

            var repository = CreateRepository(context);

            var updateDto = new UpdateCourseDto
            {
                CourseId = existing.CourseId,
                CourseName = "New",
                Cost = 200
            };

            var result = await repository.UpdateCourse(updateDto);

            Assert.NotNull(result);
            Assert.Equal("New", result.CourseName);
            Assert.Equal(200, result.Cost);
        }
    }

}
