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
using final_project_be_Infrastructure.Data;
using final_project_be_Tests.TestDAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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

        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private AssignmentRepository CreateRepository(ApplicationDbContext context)
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AssignmentRepository>();
            var mockOptions = Options.Create(new ClientSettings());
            return new AssignmentRepository(new NoTransactionAssignmentDAO(context), _mapper, logger, mockOptions);
        }

        [Fact]
        public async Task CreateAssignment_ShouldAddAssignment()
        {
            var context = GetDbContext();
            var repo = CreateRepository(context);

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
            var context = GetDbContext();
            var assignment = new Assignment { Content = "To Delete", LessonId = 1 };
            context.Assignment.Add(assignment);
            await context.SaveChangesAsync();

            var repo = CreateRepository(context);
            var result = await repo.DeleteAssignment(assignment.AssignmentId);

            Assert.True(result);
            Assert.False(context.Assignment.Any(a => a.AssignmentId == assignment.AssignmentId));
        }

        [Fact]
        public async Task GetAllAssignmentByLessonId_ShouldReturnAssignments()
        {
            var context = GetDbContext();
            context.Assignment.AddRange(
                new Assignment { LessonId = 1, Content = "A" },
                new Assignment { LessonId = 1, Content = "B" },
                new Assignment { LessonId = 2, Content = "C" }
            );
            await context.SaveChangesAsync();

            var repo = CreateRepository(context);
            var result = await repo.GetAllAssignmentByLessonId(1);

            Assert.Equal(2, result.Count);
            Assert.All(result, a => Assert.Equal(1, a.LessonId));
        }

        [Fact]
        public async Task GetAssignment_ShouldReturnCorrectAssignment()
        {
            var context = GetDbContext();
            var assignment = new Assignment { Content = "Fetch Me", LessonId = 1 };
            context.Assignment.Add(assignment);
            await context.SaveChangesAsync();

            var repo = CreateRepository(context);
            var result = await repo.GetAssignment(assignment.AssignmentId);

            Assert.NotNull(result);
            Assert.Equal("Fetch Me", result.Content);
        }

        [Fact]
        public async Task UpdateAssignment_ShouldUpdateFields()
        {
            var context = GetDbContext();
            var assignment = new Assignment { Content = "Old", LessonId = 1 };
            context.Assignment.Add(assignment);
            await context.SaveChangesAsync();

            var repo = CreateRepository(context);
            var dto = new UpdateAssignmentDto
            {
                AssignmentId = assignment.AssignmentId,
                Content = "Updated",
                LessonId = 1,
                MeetLink = "http://new.meet"
            };

            var result = await repo.UpdateAssignment(dto);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.Content);
            Assert.Equal("http://new.meet", result.MeetLink);
        }
    }

}
