using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Mentor;
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
using System.Reflection;
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
                cfg.CreateMap<Courses, GetCourseDto>();
                cfg.CreateMap<Mentor, MentorDto>()
        .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.UserMetaData.FirstName))
        .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.UserMetaData.LastName));
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

        private CourseRepository CreateRepository(ApplicationDbContext context, out Mock<IBlobStorageService> blobMock, out Mock<IOpenAIEmbeddingService> openAIMock)
        {
            blobMock = new Mock<IBlobStorageService>();
            blobMock.Setup(x => x.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>()))
                    .Returns(Task.CompletedTask);
            blobMock.Setup(x => x.DeleteFileIfExistsAsync(It.IsAny<string>()))
                    .Returns(Task.CompletedTask);
            openAIMock = new Mock<IOpenAIEmbeddingService>();
            openAIMock.Setup(x => x.GetEmbeddingAsync(It.IsAny<string>()))
                    .ReturnsAsync(new float[] { 0.1f, 0.2f, 0.3f });

            var courseDao = new NoTransactionCourseDAO(context);
            var reviewDao = new NoTransactionReviewDAO(context);
            var userCourseDao = new NoTransactionUserCourseDAO(context);

            var lessonDao = new NoTransactionLessonDAO(context);
            var courseEmbeddingDao = new NoTransactionCourseEmbeddingDAO(context);
            var moduleDao = new NoTransactionModuleDAO(context);
            var userModuleDao = new NoTransactionUserModuleDAO(context);
            var userDao = new NoTransactionUserDAO(context);
            var userEmbeddingDao = new NoTransactionUserEmbeddingDAO(context);

            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());

            var courseLogger = loggerFactory.CreateLogger<CourseRepository>();
            var userLogger = loggerFactory.CreateLogger<UserRepository>();
            var calculatorLogger = loggerFactory.CreateLogger<Caculator>();

            var calculator = new Caculator(lessonDao, moduleDao, userCourseDao, userModuleDao);
            var userRepository = new UserRepository(userDao, _mapper, userLogger);

            return new CourseRepository(
                courseDao,
                calculator,
                userCourseDao,
                reviewDao,
                _mapper,
                courseLogger,
                blobMock.Object,
                courseEmbeddingDao,
                openAIMock.Object,
                userRepository,
                userEmbeddingDao
            );
        }

        [Fact]
        public async Task CreateCourse_ShouldAddCourse()
        {
            var context = GetInMemoryDbContext();
            var repo = CreateRepository(context, out var blobMock, out var openAIMock);

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
            var mentor = new Mentor
            {
                User = new User
                {
                    Email = "mentor@example.com",
                    Password = "password123",
                    Phone = "0123456789",
                    UserMetaData = new UserMetadata
                    {
                        FirstName = "Hoang",
                        LastName = "Nguyen"
                    }
                }
            };
            context.Mentors.Add(mentor);
            await context.SaveChangesAsync();

            var course = new Courses
            {
                CourseName = "Course 1",
                CourseContent = "Content",
                CategoryId = 9,
                MentorId = mentor.MentorId,
                Requirement = "Requirement",
                IntendedLearner = "Business executives, political and civic leaders, and students",
                Language = "English",
                Level = "AllLevels",
                Cost = 450,
                SkillLearn = "Look confident, be understood",
                CourseLength = 50.1,
                Status = "Pending",
                CoursesImage = "https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/sample.png",
                Modules = new List<final_project_be_Domain.Models.Module>
        {
            new final_project_be_Domain.Models.Module
            {
                Title = "Module 1",
                Lessons = new List<Lesson>
                {
                    new Lesson { Title = "Lesson 1" },
                    new Lesson { Title = "Lesson 2" }
                }
            },
            new final_project_be_Domain.Models.Module
            {
                Title = "Module 2",
                Lessons = new List<Lesson>
                {
                    new Lesson { Title = "Lesson 3" },
                    new Lesson { Title = "Lesson 4" },
                    new Lesson { Title = "Lesson 5" }
                }
            }
        }
            };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var repo = CreateRepository(context, out var blobMock, out var openAIMock);
            var result = await repo.ToggleIsDeleted(course.CourseId);

            Assert.True(result.IsDeleted);
        }

        [Fact]
        public async Task ToggleStatus_ShouldUpdateStatus()
        {
            var context = GetInMemoryDbContext();
            var mentor = new Mentor
            {
                User = new User
                {
                    Email = "mentor@example.com",
                    Password = "password123",
                    Phone = "0123456789",
                    UserMetaData = new UserMetadata
                    {
                        FirstName = "Hoang",
                        LastName = "Nguyen"
                    }
                }
            };
            context.Mentors.Add(mentor);
            await context.SaveChangesAsync();

            var course = new Courses
            {
                CourseName = "Course 1",
                CourseContent = "Content",
                CategoryId = 9,
                MentorId = mentor.MentorId,
                Requirement = "Requirement",
                IntendedLearner = "Business executives, political and civic leaders, and students",
                Language = "English",
                Level = "AllLevels",
                Cost = 450,
                SkillLearn = "Look confident, be understood",
                CourseLength = 50.1,
                Status = "Pending",
                CoursesImage = "https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/sample.png",
                Modules = new List<final_project_be_Domain.Models.Module>
        {
            new final_project_be_Domain.Models.Module
            {
                Title = "Module 1",
                Lessons = new List<Lesson>
                {
                    new Lesson { Title = "Lesson 1" },
                    new Lesson { Title = "Lesson 2" }
                }
            },
            new final_project_be_Domain.Models.Module
            {
                Title = "Module 2",
                Lessons = new List<Lesson>
                {
                    new Lesson { Title = "Lesson 3" },
                    new Lesson { Title = "Lesson 4" },
                    new Lesson { Title = "Lesson 5" }
                }
            }
        }
            };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var repo = CreateRepository(context, out var blobMock, out var openAIMock);
            var result = await repo.ToggleStatus(course.CourseId, "Approved");

            Assert.NotNull(result);
            Assert.Equal("Approved", result.Status);
            Assert.Equal("Hoang", result.Mentor.FirstName);
            Assert.Equal("Nguyen", result.Mentor.LastName);
        }

        [Fact]
        public async Task UpdateCourse_ShouldUpdateFieldsAndReplaceImage()
        {
            var context = GetInMemoryDbContext();
            var mentor = new Mentor
            {
                User = new User
                {
                    Email = "mentor@example.com",
                    Password = "password123",
                    Phone = "0123456789",
                    UserMetaData = new UserMetadata
                    {
                        FirstName = "Hoang",
                        LastName = "Nguyen"
                    }
                }
            };
            context.Mentors.Add(mentor);
            await context.SaveChangesAsync();

            var course = new Courses
            {
                CourseName = "Course 1",
                CourseContent = "Content",
                CategoryId = 9,
                MentorId = mentor.MentorId,
                Requirement = "Requirement",
                IntendedLearner = "Business executives, political and civic leaders, and students",
                Language = "English",
                Level = "AllLevels",
                Cost = 450,
                SkillLearn = "Look confident, be understood",
                CourseLength = 50.1,
                Status = "Pending",
                CoursesImage = "https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/sample.png",
                Modules = new List<final_project_be_Domain.Models.Module>
                {
                    new final_project_be_Domain.Models.Module
                    {
                        Title = "Module 1",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Lesson 1" },
                            new Lesson { Title = "Lesson 2" }
                        }
                    },
                    new final_project_be_Domain.Models.Module
                    {
                        Title = "Module 2",
                        Lessons = new List<Lesson>
                        {
                            new Lesson { Title = "Lesson 3" },
                            new Lesson { Title = "Lesson 4" },
                            new Lesson { Title = "Lesson 5" }
                        }
                    }
                }
            };
            context.Courses.Add(course);
            await context.SaveChangesAsync();

            var repo = CreateRepository(context, out var blobMock, out var openAIMock);

            var newImage = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("newimg")), 0, 6, "NewImage", "new.jpg");

            var dto = new UpdateCourseDto
            {
                CourseId = course.CourseId,
                CourseName = "New",
                CourseContent = "New content here",
                Cost = 150,
                CoursesImage = newImage,
                CourseLength = 5,
                SkillLearn = "Skill A",
                CategoryId = 1,
                MentorId = 1,
                IntendedLearner = "Anyone",
                Level = "Beginner",
                Language = "English",
                Requirement = "None"
            };

            var result = await repo.UpdateCourse(dto);

            Assert.NotNull(result);
            Assert.Equal("New", result.CourseName);
            Assert.Equal("Pending", result.Status); // Updated to Pending on update
            Assert.StartsWith("https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/", result.CoursesImage);
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

            var repo = CreateRepository(context, out var blobMock, out var openAIMock);

            var result = repo.GetAllCourses(1, 10, null, null, null, null, null, null, null, null, null, null, null, new List<StatusEnum> { StatusEnum.Approved });

            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task GetCourse_ShouldReturnCorrectCourse()
        {
            var context = GetInMemoryDbContext();

            var mentor = new Mentor
            {
                User = new User
                {
                    Email = "mentor@example.com",
                    Password = "password123",
                    Phone = "0123456789",
                    UserMetaData = new UserMetadata
                    {
                        FirstName = "Hoang",
                        LastName = "Nguyen"
                    }
                }
            };
            context.Mentors.Add(mentor);
            await context.SaveChangesAsync();

            var course = new Courses
            {
                CourseName = "Course 1",
                CourseContent = "Content",
                CategoryId = 9,
                MentorId = mentor.MentorId,
                Requirement = "Requirement",
                IntendedLearner = "Business executives, political and civic leaders, and students",
                Language = "English",
                Level = "AllLevels",
                Cost = 450,
                SkillLearn = "Look confident, be understood",
                CourseLength = 50.1,
                Status = "Pending",
                CoursesImage = "https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/sample.png",
                Modules = new List<final_project_be_Domain.Models.Module>
        {
            new final_project_be_Domain.Models.Module
            {
                Title = "Module 1",
                Lessons = new List<Lesson>
                {
                    new Lesson { Title = "Lesson 1" },
                    new Lesson { Title = "Lesson 2" }
                }
            },
            new final_project_be_Domain.Models.Module
            {
                Title = "Module 2",
                Lessons = new List<Lesson>
                {
                    new Lesson { Title = "Lesson 3" },
                    new Lesson { Title = "Lesson 4" },
                    new Lesson { Title = "Lesson 5" }
                }
            }
        }
            };

            context.Courses.Add(course);
            await context.SaveChangesAsync();


            var repo = CreateRepository(context, out var blobMock, out var embeddingMock);

            var result = await repo.GetCourse(course.CourseId);

            Assert.NotNull(result);
            Assert.Equal("Course 1", result.CourseName);
            Assert.Equal(2, result.CountModule);
            Assert.Equal(5, result.CountLesson);
            Assert.Equal("Hoang", result.Mentor.FirstName);
        }

    }

}
