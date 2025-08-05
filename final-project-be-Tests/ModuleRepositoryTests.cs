using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Assignment;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Lesson;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Module;
using final_project_be_Domain.DTOs.Users;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace final_project_be_Tests
{
    public class ModuleRepositoryTests
    {
        private readonly Mock<IModuleDAO> _moduleDAOMock = new();
        private readonly Mock<IUserLessonDAO> _userLessonDAOMock = new();
        private readonly Mock<ILessonRepository> _lessonRepositoryMock = new();
        private readonly Mock<ICaculator> _caculatorMock = new();
        private readonly Mock<ICourseRepository> _courseRepositoryMock = new();
        private readonly Mock<IOpenAIEmbeddingService> _embeddingServiceMock = new();
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<ModuleRepository>> _loggerMock = new();
        private readonly Mock<IUserModuleDAO> _userModuleDAOMock = new();
        private readonly Mock<IModuleRepository> _moduleRepositoryMock = new();

        private readonly ModuleRepository _moduleRepo;

        public ModuleRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Module, ModuleResponseDto>().ReverseMap();
                cfg.CreateMap<ModuleDto, Module>().ReverseMap();
                cfg.CreateMap<UpdateModuleDto, Module>().ReverseMap();

                cfg.CreateMap<UpdateAssignmentDto, Assignment>().ReverseMap();
                cfg.CreateMap<Assignment, AssignmentResponseDto>().ReverseMap();

                cfg.CreateMap<Courses, CourseResponseDto>().ReverseMap();

                cfg.CreateMap<Mentor, MentorDto>().ReverseMap();
                cfg.CreateMap<UserMetadata, UserMetadataDto>().ReverseMap();
                cfg.CreateMap<User, UserDto>().ReverseMap();

                cfg.CreateMap<Lesson, LessonDto>().ReverseMap();
                cfg.CreateMap<Lesson, LessonResponseDto>();

            });


            _mapper = config.CreateMapper();

            _moduleRepo = new ModuleRepository(
                _moduleDAOMock.Object,
                _mapper,
                _loggerMock.Object,
                _userLessonDAOMock.Object,
                _userModuleDAOMock.Object,
                _caculatorMock.Object,
                _embeddingServiceMock.Object,
                _lessonRepositoryMock.Object,
                _courseRepositoryMock.Object);
        }

        [Fact]
        public async Task CreateModule_ShouldReturnCreatedModule()
        {
            // Arrange
            var moduleDto = new ModuleDto
            {
                CourseId = 1,
                Title = "Module 1",
                Description = "Desc"
            };

            _moduleDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);

            _moduleDAOMock.Setup(d => d.AddAsync(It.IsAny<Module>()))
                .Returns(Task.CompletedTask)
                .Callback<Module>(m => m.ModuleId = 1); 

            _moduleDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _moduleRepo.CreateModule(moduleDto);

            // Assert
            Assert.NotNull(result);                        
            Assert.Equal("Module 1", result.Title);        
            Assert.Equal("Desc", result.Description);
            Assert.Equal(1, result.CourseId);
            Assert.Equal(1, result.ModuleId);              
        }

        [Fact]
        public async Task GetModuleProgressByCourseAsync_ShouldReturnModuleProgressList()
        {
            var userId = Guid.NewGuid();
            int courseId = 1;
            var module = new Module
            {
                ModuleId = 1,
                Title = "Module 1",
                Description = "Desc",
                Lessons = new List<Lesson> { new Lesson { LessonId = 10, Title = "Lesson 1" } }
            };

            _moduleDAOMock.Setup(d => d.GetModulesWithLessonsByCourseIdAsync(courseId)).ReturnsAsync(new List<Module> { module });
            _caculatorMock.Setup(c => c.CalculateModuleProgress(userId, module.ModuleId)).ReturnsAsync(80);
            _userLessonDAOMock.Setup(d => d.GetUserLessonsByModuleAsync(userId, module.ModuleId))
                .ReturnsAsync(new List<UserLesson> { new UserLesson { LessonId = 10, IsPassed = true } });

            var result = await _moduleRepo.GetModuleProgressByCourseAsync(userId, courseId);

            Assert.Single(result);
            Assert.Equal(80, result[0].Percentage);
            Assert.True(result[0].Lessons[0].Ispassed);
        }

        [Fact]
        public async Task DeleteModule_ShouldReturnTrue_WhenSuccess()
        {
            int moduleId = 1;

            _moduleDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _moduleDAOMock.Setup(d => d.DeleteAsync(moduleId)).Returns(Task.CompletedTask);
            _moduleDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _moduleRepo.DeleteModule(moduleId);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateModule_ShouldUpdateAndReturnModule()
        {
            // Arrange
            var dto = new UpdateModuleDto { ModuleId = 1, Title = "Updated" };
            var module = new Module { ModuleId = 1, Title = "Old" };

            _moduleDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _moduleDAOMock.Setup(d => d.GetByIdAsync(dto.ModuleId)).ReturnsAsync(module);
            _moduleDAOMock.Setup(d => d.UpdateAsync(It.IsAny<Module>())).Returns(Task.CompletedTask);
            _moduleDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _moduleRepo.UpdateModule(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.ModuleId, result.ModuleId);
            Assert.Equal(dto.Title, result.Title);
        }

        [Fact]
        public async Task GenerateAndSaveModulesAsync_ShouldCreateModulesAndLessons()
        {
            int courseId = 1;
            var course = new Courses
            {
                CourseId = courseId,
                Status = "Pending",
                Mentor = new Mentor
                {
                    User = new User
                    {
                        UserMetaData = new UserMetadata { FirstName = "Hoang", LastName = "Nguyen" }
                    }
                }
            };

            var aiModules = new List<AIGeneratedModule>
            {
                new AIGeneratedModule
                {
                    Title = "Module 1",
                    Description = "Desc",
                    Lessons = new List<AIGeneratedLesson>
                    {
                        new AIGeneratedLesson { Title = "Lesson 1", Description = "Lesson Desc" }
                    }
                }
            };

            var courseResponseDto = _mapper.Map<CourseResponseDto>(course);

            _courseRepositoryMock.Setup(repo => repo.GetCourse(It.IsAny<int>()))
                .ReturnsAsync(courseResponseDto);

            _embeddingServiceMock.Setup(s => s.GetChatCompletionAsync(It.IsAny<string>()))
                .ReturnsAsync(JsonSerializer.Serialize(aiModules));

            _moduleRepositoryMock.Setup(r => r.CreateModule(It.IsAny<ModuleDto>()))
                .ReturnsAsync(new Module { ModuleId = 1 });

            _lessonRepositoryMock.Setup(r => r.CreateLesson(It.IsAny<LessonDto>())).ReturnsAsync(new Lesson());

            var result = await _moduleRepo.GenerateAndSaveModulesAsync(courseId);

            Assert.True(result);
        }
        [Fact]
        public async Task GetModule_ShouldReturnCorrectModule()
        {
            var moduleId = 1;

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

            var module = new Module
            {
                CourseId = 1,
                Courses = course,
                ModuleId = moduleId,
                IsPremium = true,
                Title = "Test Module",
                Description = "Desc",
                Lessons = new List<Lesson>
        {
            new Lesson { LessonId = 1, Title = "Lesson 1", ModuleId = moduleId },
            new Lesson { LessonId = 2, Title = "Lesson 2", ModuleId = moduleId }
        }
            };

            _moduleDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _moduleDAOMock.Setup(d => d.GetByIdAsync(moduleId)).ReturnsAsync(module);
            _moduleDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _moduleRepo.GetModule(moduleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(moduleId, result.ModuleId);
            Assert.Equal("Test Module", result.Title);
            Assert.Equal(2, result.CountLesson);
        }

    }
}
