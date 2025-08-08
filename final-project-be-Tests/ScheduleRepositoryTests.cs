using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Schedule;
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
    public class ScheduleRepositoryTests
    {
        private readonly Mock<IScheduleDAO> _scheduleDAOMock;
        private readonly Mock<IUserScheduleDAO> _userScheduleDAOMock;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<AnswerRepository>> _loggerMock;
        private readonly ScheduleRepository _repository;

        public ScheduleRepositoryTests()
        {
            _scheduleDAOMock = new Mock<IScheduleDAO>();
            _userScheduleDAOMock = new Mock<IUserScheduleDAO>();
            _loggerMock = new Mock<ILogger<AnswerRepository>>();

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ScheduleDto, Schedule>().ReverseMap();
            });
            _mapper = mapperConfig.CreateMapper();

            _repository = new ScheduleRepository(
                _scheduleDAOMock.Object,
                _mapper,
                _loggerMock.Object,
                _userScheduleDAOMock.Object
            );
        }

        [Fact]
        public async Task CreateScheduleAsync_ValidDto_ReturnsTrue()
        {
            // Arrange
            var dto = new ScheduleDto
            {
                ScheduleId = 1,
                MentorId = 100,
                ScheduleName = "Morning Session",
                MentorDay = DateTime.Today,
                CreateAt = DateTime.Today.AddDays(-1),
                CourseId = 200
            };

            _scheduleDAOMock.Setup(d => d.AddScheduleAsync(It.IsAny<Schedule>()))
                            .Returns(Task.CompletedTask);
            _scheduleDAOMock.Setup(d => d.SaveChangesAsync())
                            .Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateScheduleAsync(dto);

            // Assert
            Assert.True(result);
            _scheduleDAOMock.Verify(d => d.AddScheduleAsync(It.Is<Schedule>(
                s => s.ScheduleId == dto.ScheduleId &&
                     s.MentorId == dto.MentorId &&
                     s.ScheduleName == dto.ScheduleName &&
                     s.MentorDay == dto.MentorDay &&
                     s.CreateAt == dto.CreateAt &&
                     s.CourseId == dto.CourseId
            )), Times.Once);
            _scheduleDAOMock.Verify(d => d.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateScheduleAsync_EmptyScheduleName_StillReturnsTrue()
        {
            // Arrange
            var dto = new ScheduleDto
            {
                ScheduleId = 2,
                MentorId = 101,
                ScheduleName = "", 
                MentorDay = DateTime.Today,
                CreateAt = DateTime.Now,
                CourseId = 201
            };

            _scheduleDAOMock.Setup(d => d.AddScheduleAsync(It.IsAny<Schedule>()))
                            .Returns(Task.CompletedTask);
            _scheduleDAOMock.Setup(d => d.SaveChangesAsync())
                            .Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateScheduleAsync(dto);

            // Assert
            Assert.True(result);
            _scheduleDAOMock.Verify(d => d.AddScheduleAsync(It.Is<Schedule>(
                s => s.ScheduleName == dto.ScheduleName
            )), Times.Once);
            _scheduleDAOMock.Verify(d => d.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateScheduleAsync_ExceptionThrown_ThrowsException()
        {
            // Arrange
            var dto = new ScheduleDto
            {
                ScheduleId = 3,
                MentorId = 102,
                ScheduleName = "Evening Class",
                MentorDay = DateTime.Today.AddDays(1),
                CreateAt = DateTime.Now,
                CourseId = 202
            };

            _scheduleDAOMock.Setup(d => d.AddScheduleAsync(It.IsAny<Schedule>()))
                            .ThrowsAsync(new Exception("DB error"));

            // Act
            var ex = await Record.ExceptionAsync(() => _repository.CreateScheduleAsync(dto));

            // Assert
            Assert.NotNull(ex);
            var exception = Assert.IsType<Exception>(ex);
            Assert.Equal("DB error", exception.Message);

            _scheduleDAOMock.Verify(d => d.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RegisterUserToScheduleAsync_ScheduleNotFound_ReturnsFalse()
        {
            // Arrange
            var dto = new UserScheduleDto { UserId = Guid.NewGuid(), ScheduleId = 1 };
            _scheduleDAOMock.Setup(d => d.GetByIdAsync(dto.ScheduleId)).ReturnsAsync((Schedule)null);

            // Act
            var result = await _repository.RegisterUserToScheduleAsync(dto);

            // Assert
            Assert.False(result);
            _userScheduleDAOMock.Verify(u => u.AddAsync(It.IsAny<UserSchedule>()), Times.Never);
        }

        [Fact]
        public async Task RegisterUserToScheduleAsync_UserNotEnrolled_ReturnsFalse()
        {
            // Arrange
            var dto = new UserScheduleDto { UserId = Guid.NewGuid(), ScheduleId = 1 };
            var schedule = new Schedule { ScheduleId = 1, CourseId = 100 };

            _scheduleDAOMock.Setup(d => d.GetByIdAsync(dto.ScheduleId)).ReturnsAsync(schedule);
            _scheduleDAOMock.Setup(d => d.HasUserEnrolledCourseAsync(schedule.CourseId, dto.UserId)).ReturnsAsync(false);

            // Act
            var result = await _repository.RegisterUserToScheduleAsync(dto);

            // Assert
            Assert.False(result);
            _userScheduleDAOMock.Verify(u => u.AddAsync(It.IsAny<UserSchedule>()), Times.Never);
        }

        [Fact]
        public async Task RegisterUserToScheduleAsync_UserAlreadyRegistered_ReturnsFalse()
        {
            // Arrange
            var dto = new UserScheduleDto { UserId = Guid.NewGuid(), ScheduleId = 1 };
            var schedule = new Schedule { ScheduleId = 1, CourseId = 100 };

            _scheduleDAOMock.Setup(d => d.GetByIdAsync(dto.ScheduleId)).ReturnsAsync(schedule);
            _scheduleDAOMock.Setup(d => d.HasUserEnrolledCourseAsync(schedule.CourseId, dto.UserId)).ReturnsAsync(true);
            _scheduleDAOMock.Setup(d => d.IsUserAlreadyRegisteredAsync(dto.UserId, dto.ScheduleId)).ReturnsAsync(true);

            // Act
            var result = await _repository.RegisterUserToScheduleAsync(dto);

            // Assert
            Assert.False(result);
            _userScheduleDAOMock.Verify(u => u.AddAsync(It.IsAny<UserSchedule>()), Times.Never);
        }

        [Fact]
        public async Task RegisterUserToScheduleAsync_ValidData_ReturnsTrue()
        {
            // Arrange
            var dto = new UserScheduleDto { UserId = Guid.NewGuid(), ScheduleId = 1 };
            var schedule = new Schedule { ScheduleId = 1, CourseId = 100 };

            _scheduleDAOMock.Setup(d => d.GetByIdAsync(dto.ScheduleId)).ReturnsAsync(schedule);
            _scheduleDAOMock.Setup(d => d.HasUserEnrolledCourseAsync(schedule.CourseId, dto.UserId)).ReturnsAsync(true);
            _scheduleDAOMock.Setup(d => d.IsUserAlreadyRegisteredAsync(dto.UserId, dto.ScheduleId)).ReturnsAsync(false);

            // Act
            var result = await _repository.RegisterUserToScheduleAsync(dto);

            // Assert
            Assert.True(result);
            _userScheduleDAOMock.Verify(u => u.AddAsync(It.Is<UserSchedule>(us =>
                us.UserId == dto.UserId && us.ScheduleId == dto.ScheduleId
            )), Times.Once);
            _scheduleDAOMock.Verify(d => d.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteSchedule_Success_ReturnsTrue_AndCommits()
        {
            // Arrange
            int scheduleId = 1;

            // Act
            var result = await _repository.DeleteSchedule(scheduleId);

            // Assert
            Assert.True(result);
            _scheduleDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _scheduleDAOMock.Verify(d => d.DeleteAsync(scheduleId), Times.Once);
            _scheduleDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _scheduleDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Never);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Delete Schedule success")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task DeleteSchedule_ExceptionThrown_ReturnsFalse_AndRollbacks()
        {
            // Arrange
            int scheduleId = 1;
            _scheduleDAOMock.Setup(d => d.DeleteAsync(scheduleId)).ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _repository.DeleteSchedule(scheduleId);

            // Assert
            Assert.False(result);
            _scheduleDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _scheduleDAOMock.Verify(d => d.DeleteAsync(scheduleId), Times.Once);
            _scheduleDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Never);
            _scheduleDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);

            _loggerMock.Verify(
                l => l.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((o, t) => o.ToString().Contains("Error when delete Schedule")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once
            );
        }

        [Fact]
        public async Task GetSchedulesByCourseAsync_WithData_ReturnsMappedDtos()
        {
            // Arrange
            int courseId = 1;
            var schedules = new List<Schedule>
        {
            new Schedule
            {
                ScheduleId = 101,
                MentorId = 10,
                ScheduleName = "Morning Session",
                MentorDay = new DateTime(2025, 8, 10),
                CreateAt = DateTime.UtcNow,
                CourseId = courseId
            },
            new Schedule
            {
                ScheduleId = 102,
                MentorId = 11,
                ScheduleName = "Evening Session",
                MentorDay = new DateTime(2025, 8, 11),
                CreateAt = DateTime.UtcNow,
                CourseId = courseId
            }
        };

            _scheduleDAOMock.Setup(d => d.GetSchedulesByCourseIdAsync(courseId))
                .ReturnsAsync(schedules);

            // Act
            var result = await _repository.GetSchedulesByCourseAsync(courseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Morning Session", result[0].ScheduleName);
            Assert.Equal(10, result[0].MentorId);
            Assert.Equal("Evening Session", result[1].ScheduleName);
            Assert.Equal(11, result[1].MentorId);
        }

        [Fact]
        public async Task GetSchedulesByCourseAsync_NoData_ReturnsEmptyList()
        {
            // Arrange
            int courseId = 1;
            _scheduleDAOMock.Setup(d => d.GetSchedulesByCourseIdAsync(courseId))
                .ReturnsAsync(new List<Schedule>());

            // Act
            var result = await _repository.GetSchedulesByCourseAsync(courseId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetSchedulesByMentorAsync_ReturnsMappedList()
        {
            // Arrange
            var mentorId = 1;

            var schedules = new List<Schedule>
    {
        new Schedule { ScheduleId = 1, MentorId = mentorId, ScheduleName = "Test Schedule" }
    };

            var mockScheduleDao = new Mock<IScheduleDAO>();
            mockScheduleDao.Setup(d => d.GetSchedulesByMentorIdAsync(mentorId))
                           .ReturnsAsync(schedules);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Schedule, ScheduleDto>();
            });
            var mapper = mapperConfig.CreateMapper();

            var repo = new ScheduleRepository(
                mockScheduleDao.Object,
                mapper,
                Mock.Of<ILogger<AnswerRepository>>(),
                Mock.Of<IUserScheduleDAO>()
            );

            // Act
            var result = await repo.GetSchedulesByMentorAsync(mentorId);

            // Assert
            Assert.Single(result);
            Assert.Equal("Test Schedule", result[0].ScheduleName);
        }

        [Fact]
        public async Task GetUserSchedulesAsync_ReturnsMappedDtos()
        {
            // Arrange
            var userId = Guid.NewGuid();

            var userSchedules = new List<UserSchedule>
    {
        new UserSchedule { UserId = userId, ScheduleId = 1 },
        new UserSchedule { UserId = userId, ScheduleId = 2 }
    };

            var userScheduleDAOMock = new Mock<IUserScheduleDAO>();
            userScheduleDAOMock.Setup(d => d.GetSchedulesByUserIdAsync(userId))
                               .ReturnsAsync(userSchedules);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserSchedule, UserScheduleDto>();
            });
            var mapper = mapperConfig.CreateMapper();

            var repo = new ScheduleRepository(
                Mock.Of<IScheduleDAO>(),
                mapper,
                Mock.Of<ILogger<AnswerRepository>>(),
                userScheduleDAOMock.Object
            );

            // Act
            var result = await repo.GetUserSchedulesAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, dto => Assert.Equal(userId, dto.UserId));
        }

    }
}
