using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Assignment;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace final_project_be_Tests
{
    public class AssignmentRepositoryTests
    {
        private readonly IMapper _mapper;

        public AssignmentRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AssignmentDto, Assignment>();
                cfg.CreateMap<UpdateAssignmentDto, Assignment>();
                cfg.CreateMap<Assignment, AssignmentResponseDto>();
            });

            _mapper = config.CreateMapper();
        }

        private AssignmentRepository CreateRepository(Mock<IAssignmentDAO> mockDao, List<Assignment> assignments)
        {
            var logger = Mock.Of<ILogger<AssignmentRepository>>();
            var options = Options.Create(new ClientSettings());

            mockDao.Setup(d => d.GetAll()).Returns(assignments.AsQueryable());

            return new AssignmentRepository(mockDao.Object, _mapper, logger, options);
        }

        [Fact]
        public async Task CreateAssignment_ShouldAddAssignment()
        {
            var mockDao = new Mock<IAssignmentDAO>();
            var assignments = new List<Assignment>();

            mockDao.Setup(d => d.AddAsync(It.IsAny<Assignment>()))
                   .Callback<Assignment>(a => { a.AssignmentId = 1; assignments.Add(a); })
                   .Returns(Task.CompletedTask);

            var repo = CreateRepository(mockDao, assignments);

            var dto = new AssignmentDto
            {
                LessonId = 1,
                Content = "New Assignment",
                MeetLink = "http://meet.link"
            };

            var result = await repo.CreateAssignment(dto);

            Assert.NotNull(result);
            Assert.Equal("New Assignment", result.Content);
            Assert.Equal("http://meet.link", result.MeetLink);
        }

        [Fact]
        public async Task DeleteAssignment_ShouldRemoveAssignment()
        {
            // Arrange
            var mockDao = new Mock<IAssignmentDAO>();
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AssignmentRepository>();
            var mockOptions = Options.Create(new ClientSettings());

            var assignmentId = 1;
            var deleted = false;

            mockDao.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.DeleteAsync(assignmentId))
                   .Callback(() => deleted = true)
                   .Returns(Task.CompletedTask);
            mockDao.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var repo = new AssignmentRepository(mockDao.Object, _mapper, logger, mockOptions);

            // Act
            var result = await repo.DeleteAssignment(assignmentId);

            // Assert
            Assert.True(result);
            Assert.True(deleted);
        }

        [Fact]
        public async Task GetAllAssignmentByLessonId_ShouldReturnAssignments()
        {
            var assignments = new List<Assignment>
        {
            new Assignment { AssignmentId = 1, LessonId = 1, Content = "A" },
            new Assignment { AssignmentId = 2, LessonId = 1, Content = "B" },
            new Assignment { AssignmentId = 3, LessonId = 2, Content = "C" }
        };

            var mockDao = new Mock<IAssignmentDAO>();
            var repo = CreateRepository(mockDao, assignments);

            var result = await repo.GetAllAssignmentByLessonId(1);

            Assert.Equal(2, result.Count);
            Assert.All(result, a => Assert.Equal(1, a.LessonId));
        }

        [Fact]
        public async Task GetAssignment_ShouldReturnCorrectAssignment()
        {
            var assignment = new Assignment { AssignmentId = 1, LessonId = 1, Content = "Fetch Me" };
            var mockDao = new Mock<IAssignmentDAO>();
            mockDao.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(assignment);

            var repo = CreateRepository(mockDao, new List<Assignment>());

            var result = await repo.GetAssignment(1);

            Assert.NotNull(result);
            Assert.Equal("Fetch Me", result.Content);
        }

        [Fact]
        public async Task UpdateAssignment_ShouldUpdateFields()
        {
            var assignment = new Assignment { AssignmentId = 1, LessonId = 1, Content = "Old", MeetLink = null };

            var mockDao = new Mock<IAssignmentDAO>();
            mockDao.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(assignment);
            mockDao.Setup(d => d.UpdateAsync(It.IsAny<Assignment>()))
                   .Callback<Assignment>(a =>
                   {
                       assignment.Content = a.Content;
                       assignment.MeetLink = a.MeetLink;
                   })
                   .Returns(Task.CompletedTask);

            var repo = CreateRepository(mockDao, new List<Assignment>());

            var dto = new UpdateAssignmentDto
            {
                AssignmentId = 1,
                LessonId = 1,
                Content = "Updated",
                MeetLink = "http://new.meet"
            };

            var result = await repo.UpdateAssignment(dto);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.Content);
            Assert.Equal("http://new.meet", result.MeetLink);
        }
    }

}
