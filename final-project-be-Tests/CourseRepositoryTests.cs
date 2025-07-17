using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace final_project_be_Tests
{
    public class CourseRepositoryTests
    {
        private readonly Mock<ICourseDAO> _courseDAOMock = new();
        private readonly Mock<ICourseEmbeddingDAO> _embeddingDAOMock = new();
        private readonly Mock<IOpenAIEmbeddingService> _embeddingServiceMock = new();
        private readonly Mock<IUserCourseDAO> _userCourseDAOMock = new();
        private readonly Mock<IReviewDAO> _reviewDAOMock = new();
        private readonly Mock<IBlobStorageService> _blobStorageMock = new();
        private readonly Mock<IUserRepository> _userRepoMock = new();
        private readonly Mock<IUserEmbeddingDAO> _userEmbeddingDAOMock = new();
        private readonly Mock<ILogger<CourseRepository>> _loggerMock = new();

        private readonly Mock<ILessonDAO> _lessonDAOMock = new();
        private readonly Mock<IModuleDAO> _moduleDAOMock = new();
        private readonly Mock<IUserModuleDAO> _userModuleDAOMock = new();

        private readonly Mock<ICaculator> _caculatorMock = new();

        private readonly IMapper _mapper;

        public CourseRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CourseDto, Courses>();
                cfg.CreateMap<UpdateCourseDto, Courses>();
                cfg.CreateMap<Courses, CourseResponseDto>();
                cfg.CreateMap<Mentor, MentorDto>()
                    .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.UserMetaData.FirstName))
                    .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.UserMetaData.LastName));
                cfg.CreateMap<Courses, CourseResponseDto>()
                    .ForMember(dest => dest.Mentor, opt => opt.MapFrom(src => src.Mentor));
            });

            _mapper = config.CreateMapper();
        }


        [Fact]
        public async Task CreateCourse_ShouldUploadImageAndSaveCourse()
        {
            // Arrange
            var dto = new CourseDto
            {
                CourseName = "Test Course",
                CourseContent = "AI basics",
                CoursesImage = GetMockFormFile("image.jpg")
            };

            var courseRepository = new CourseRepository(
                _courseDAOMock.Object,
                _caculatorMock.Object,
                _userCourseDAOMock.Object,
                _reviewDAOMock.Object,
                _mapper,
                _loggerMock.Object,
                _blobStorageMock.Object,
                _embeddingDAOMock.Object,
                _embeddingServiceMock.Object,
                _userRepoMock.Object,
                _userEmbeddingDAOMock.Object
            );

            _embeddingServiceMock.Setup(s => s.GetEmbeddingAsync(It.IsAny<string>()))
                .ReturnsAsync(new float[] { 0.1f, 0.2f });

            _courseDAOMock.Setup(d => d.AddAsync(It.IsAny<Courses>())).Returns(Task.CompletedTask);
            _courseDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _courseDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            _blobStorageMock.Setup(b => b.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.CompletedTask);
            _embeddingDAOMock.Setup(e => e.AddAsync(It.IsAny<CourseEmbedding>())).Returns(Task.CompletedTask);

            // Act
            var result = await courseRepository.CreateCourse(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Course", result.CourseName);
            _blobStorageMock.Verify(b => b.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once);
            _courseDAOMock.Verify(d => d.AddAsync(It.IsAny<Courses>()), Times.Once);
        }

        private IFormFile GetMockFormFile(string fileName)
        {
            var content = "Fake image content";
            var fileStream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content));
            return new FormFile(fileStream, 0, fileStream.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }

        [Fact]
        public async Task ToggleIsDeleted_ShouldSwitchFlag()
        {
            // Arrange
            var course = new Courses
            {
                CourseId = 1,
                IsDeleted = false
            };

            _courseDAOMock.Setup(d => d.GetByIdAsync(course.CourseId)).ReturnsAsync(course);
            _courseDAOMock.Setup(d => d.UpdateAsync(course)).Returns(Task.CompletedTask);

            var courseRepo = new CourseRepository(
                _courseDAOMock.Object,
                _caculatorMock.Object,
                _userCourseDAOMock.Object,
                _reviewDAOMock.Object,
                _mapper,
                _loggerMock.Object,
                _blobStorageMock.Object,
                _embeddingDAOMock.Object,
                _embeddingServiceMock.Object,
                _userRepoMock.Object,
                _userEmbeddingDAOMock.Object
            );

            // Act
            var result = await courseRepo.ToggleIsDeleted(course.CourseId);

            // Assert
            Assert.True(result.IsDeleted);
            _courseDAOMock.Verify(d => d.UpdateAsync(It.Is<Courses>(c => c.IsDeleted)), Times.Once);
        }

        [Fact]
        public async Task ToggleStatus_ShouldUpdateStatus()
        {
            // Arrange
            var course = new Courses
            {
                CourseId = 1,
                Status = "Pending",
                Mentor = new Mentor
                {
                    User = new User
                    {
                        UserMetaData = new UserMetadata { FirstName = "Hoang", LastName = "Nguyen" }
                    }
                }
            };

            _courseDAOMock.Setup(d => d.GetByIdAsync(course.CourseId)).ReturnsAsync(course);
            _courseDAOMock.Setup(d => d.UpdateAsync(It.IsAny<Courses>())).Returns(Task.CompletedTask);

            var repo = new CourseRepository(
                _courseDAOMock.Object,
                _caculatorMock.Object,
                _userCourseDAOMock.Object,
                _reviewDAOMock.Object,
                _mapper,
                _loggerMock.Object,
                _blobStorageMock.Object,
                _embeddingDAOMock.Object,
                _embeddingServiceMock.Object,
                _userRepoMock.Object,
                _userEmbeddingDAOMock.Object
            );

            // Act
            var result = await repo.ToggleStatus(course.CourseId, "Approved");

            // Assert
            Assert.Equal("Approved", result.Status);
            Assert.Equal("Hoang", result.Mentor.FirstName);
        }

        [Fact]
        public async Task UpdateCourse_ShouldUpdateFieldsAndReplaceImage()
        {
            // Arrange
            var existingCourse = new Courses
            {
                CourseId = 1,
                CourseName = "Old Name",
                CoursesImage = "https://old-url.com/image.png"
            };

            var newImage = GetMockFormFile("new.jpg");

            var updateDto = new UpdateCourseDto
            {
                CourseId = 1,
                CourseName = "Updated Name",
                CoursesImage = newImage,
                CourseContent = "Updated Content",
                Cost = 200,
                CourseLength = 10,
                SkillLearn = "Updated Skill",
                CategoryId = 1,
                MentorId = 1,
                IntendedLearner = "Everyone",
                Level = "Beginner",
                Language = "English",
                Requirement = "None"
            };

            _courseDAOMock.Setup(d => d.GetByIdAsync(updateDto.CourseId)).ReturnsAsync(existingCourse);
            _courseDAOMock.Setup(d => d.UpdateAsync(It.IsAny<Courses>())).Returns(Task.CompletedTask);
            _blobStorageMock.Setup(b => b.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.CompletedTask);
            _blobStorageMock.Setup(b => b.DeleteFileIfExistsAsync(It.IsAny<string>())).Returns(Task.CompletedTask);

            var repo = new CourseRepository(
                _courseDAOMock.Object,
                _caculatorMock.Object,
                _userCourseDAOMock.Object,
                _reviewDAOMock.Object,
                _mapper,
                _loggerMock.Object,
                _blobStorageMock.Object,
                _embeddingDAOMock.Object,
                _embeddingServiceMock.Object,
                _userRepoMock.Object,
                _userEmbeddingDAOMock.Object
            );

            // Act
            var result = await repo.UpdateCourse(updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.CourseName);
            _blobStorageMock.Verify(b => b.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once);
        }

        [Fact]
        public void GetAllCourses_ShouldReturnFilteredPaginatedCourses()
        {
            // Arrange
            var now = DateTime.Now;

            var fakeCourses = new List<Courses>
    {
        new Courses
        {
            CourseId = 1,
            CourseName = "A",
            Cost = 100,
            Status = "Approved",
            CreateAt = now,
            Mentor = new Mentor
            {
                User = new User
                {
                    UserMetaData = new UserMetadata { FirstName = "Test", LastName = "User" }
                }
            },
            Reviews = new List<Review>(),
        },
        new Courses
        {
            CourseId = 2,
            CourseName = "B",
            Cost = 200,
            Status = "Approved",
            CreateAt = now,
            Mentor = new Mentor
            {
                User = new User
                {
                    UserMetaData = new UserMetadata { FirstName = "Test", LastName = "User" }
                }
            },
            Reviews = new List<Review>(),
        },
        new Courses
        {
            CourseId = 3,
            CourseName = "C",
            Cost = 300,
            Status = "Pending",
            CreateAt = now,
            Mentor = new Mentor
            {
                User = new User
                {
                    UserMetaData = new UserMetadata { FirstName = "Test", LastName = "User" }
                }
            },
            Reviews = new List<Review>(),
        }
    }.AsQueryable();

            _courseDAOMock.Setup(d => d.GetAll()).Returns(fakeCourses);

            var repo = new CourseRepository(
                _courseDAOMock.Object,
                _caculatorMock.Object,
                _userCourseDAOMock.Object,
                _reviewDAOMock.Object,
                _mapper,
                _loggerMock.Object,
                _blobStorageMock.Object,
                _embeddingDAOMock.Object,
                _embeddingServiceMock.Object,
                _userRepoMock.Object,
                _userEmbeddingDAOMock.Object
            );

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
                statuses: new List<StatusEnum> { StatusEnum.Approved }
            );

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task GetCourse_ShouldReturnCorrectCourse()
        {
            // Arrange
            var course = new Courses
            {
                CourseId = 1,
                CourseName = "Course 1",
                Mentor = new Mentor
                {
                    User = new User
                    {
                        UserMetaData = new UserMetadata
                        {
                            FirstName = "Hoang",
                            LastName = "Nguyen"
                        }
                    }
                },
                Modules = new List<final_project_be_Domain.Models.Module>
        {
            new final_project_be_Domain.Models.Module { Lessons = new List<Lesson> { new Lesson(), new Lesson() } },
            new final_project_be_Domain.Models.Module { Lessons = new List<Lesson> { new Lesson(), new Lesson(), new Lesson() } }
        }
            };

            _courseDAOMock.Setup(d => d.GetByIdAsync(course.CourseId))
                          .ReturnsAsync(course);

            var repo = new CourseRepository(
                _courseDAOMock.Object,
                _caculatorMock.Object,
                _userCourseDAOMock.Object,
                _reviewDAOMock.Object,
                _mapper,
                _loggerMock.Object,
                _blobStorageMock.Object,
                _embeddingDAOMock.Object,
                _embeddingServiceMock.Object,
                _userRepoMock.Object,
                _userEmbeddingDAOMock.Object
            );

            // Act
            var result = await repo.GetCourse(course.CourseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Course 1", result.CourseName);
            Assert.Equal(2, result.CountModule);
            Assert.Equal(5, result.CountLesson);
            Assert.Equal("Hoang", result.Mentor.FirstName);
        }

        [Fact]
        public async Task RecommendCoursesAsync_ShouldReturnTopRecommendedCourses()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var inputEmbedding = new float[] { 0.1f, 0.2f, 0.3f };

            var courseEmbeddings = new List<CourseEmbedding>
{
    new CourseEmbedding
    {
        CourseId = 1,
        EmbeddingJson = JsonConvert.SerializeObject(new float[] { 0.1f, 0.2f, 0.3f }),
        Course = new Courses
        {
            CourseId = 1,
            CourseName = "Course A",
            Status = "Approved",
            IsDeleted = false,
            CategoryId = 1,
            Reviews = new List<Review>(),
            Mentor = new Mentor
            {
                User = new User
                {
                    UserMetaData = new UserMetadata
                    {
                        FirstName = "John",
                        LastName = "Doe"
                    }
                }
            }
        }
    },
    new CourseEmbedding
    {
        CourseId = 2,
        EmbeddingJson = JsonConvert.SerializeObject(new float[] { 0.3f, 0.2f, 0.1f }),
        Course = new Courses
        {
            CourseId = 2,
            CourseName = "Course B",
            Status = "Approved",
            IsDeleted = false,
            CategoryId = 2,
            Reviews = new List<Review>(),
            Mentor = new Mentor
            {
                User = new User
                {
                    UserMetaData = new UserMetadata
                    {
                        FirstName = "Alice",
                        LastName = "Smith"
                    }
                }
            }
        }
    },
    new CourseEmbedding
    {
        CourseId = 3,
        EmbeddingJson = JsonConvert.SerializeObject(new float[] { 0.9f, 0.9f, 0.9f }),
        Course = new Courses
        {
            CourseId = 3,
            CourseName = "Course C",
            Status = "Approved",
            IsDeleted = false,
            CategoryId = 2,
            Reviews = new List<Review>(),
            Mentor = new Mentor
            {
                User = new User
                {
                    UserMetaData = new UserMetadata
                    {
                        FirstName = "Bob",
                        LastName = "Marley"
                    }
                }
            }
        }
    }
};

            // Mock embedding service
            _embeddingServiceMock.Setup(x => x.GetEmbeddingAsync(It.IsAny<string>()))
                .ReturnsAsync(inputEmbedding);

            // Mock embedding DAO
            _embeddingDAOMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(courseEmbeddings);

            // Mock cosine similarity
            _caculatorMock.Setup(x => x.CosineSimilarity(inputEmbedding, It.Is<float[]>(a => a.SequenceEqual(new float[] { 0.1f, 0.2f, 0.3f }))))
                .Returns(0.99);

            _caculatorMock.Setup(x => x.CosineSimilarity(inputEmbedding, It.Is<float[]>(a => a.SequenceEqual(new float[] { 0.3f, 0.2f, 0.1f }))))
                .Returns(0.8);

            _caculatorMock.Setup(x => x.CosineSimilarity(inputEmbedding, It.Is<float[]>(a => a.SequenceEqual(new float[] { 0.9f, 0.9f, 0.9f }))))
                .Returns(0.5);

            // Mock GetByIdAsync
            _courseDAOMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(new Courses { CourseId = 1, CourseName = "Course A", Status = "Approved" });
            _courseDAOMock.Setup(x => x.GetByIdAsync(2)).ReturnsAsync(new Courses { CourseId = 2, CourseName = "Course B", Status = "Approved" });
            _courseDAOMock.Setup(x => x.GetByIdAsync(3)).ReturnsAsync(new Courses { CourseId = 3, CourseName = "Course C", Status = "Approved" });

            _courseDAOMock.Setup(x => x.GetAll()).Returns(new List<Courses>
{
    new Courses
    {
        CourseId = 1,
        CourseName = "Course A",
        Status = "Approved",
        IsDeleted = false,
        CategoryId = 10,
        Reviews = new List<Review>(),
        Mentor = new Mentor
        {
            User = new User
            {
                UserMetaData = new UserMetadata
                {
                    FirstName = "John",
                    LastName = "Doe"
                }
            }
        }
    },
    new Courses
    {
        CourseId = 2,
        CourseName = "Course B",
        Status = "Approved",
        IsDeleted = false,
        CategoryId = 20,
        Reviews = new List<Review>(),
        Mentor = new Mentor
        {
            User = new User
            {
                UserMetaData = new UserMetadata
                {
                    FirstName = "Jane",
                    LastName = "Smith"
                }
            }
        }
    },
    new Courses
    {
        CourseId = 3,
        CourseName = "Course C",
        Status = "Approved",
        IsDeleted = false,
        CategoryId = 10,
        Reviews = new List<Review>(),
        Mentor = new Mentor
        {
            User = new User
            {
                UserMetaData = new UserMetadata
                {
                    FirstName = "Alice",
                    LastName = "Brown"
                }
            }
        }
    }
}.AsQueryable());
            _userCourseDAOMock.Setup(x => x.GetUserCoursesByUserId(It.IsAny<Guid>()))
    .ReturnsAsync(new List<UserCourse>
    {
        new UserCourse
        {
            CourseId = 1,
            UserId = userId,
            Status = "Completed"
        },
        new UserCourse
        {
            CourseId = 2,
            UserId = userId,
            Status = "Pending"
        }
    });

            // Create repository
            var repository = new CourseRepository(
                _courseDAOMock.Object,
                _caculatorMock.Object,
                _userCourseDAOMock.Object,
                _reviewDAOMock.Object,
                _mapper,
                _loggerMock.Object,
                _blobStorageMock.Object,
                _embeddingDAOMock.Object,
                _embeddingServiceMock.Object,
                _userRepoMock.Object,
                _userEmbeddingDAOMock.Object
            );

            // Act
            var result = await repository.RecommendCoursesAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Count.Should().Be(1);
            result[0].CourseId.Should().Be(3);
        }


    }

}
