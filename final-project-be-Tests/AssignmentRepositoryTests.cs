using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Assignment;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.Data;
using final_project_be_Tests.TestDAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateAssignment_ShouldAddAssignment()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionAssignmentDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AssignmentRepository>();
            var repository = new AssignmentRepository(dao, _mapper, logger);

            var dto = new AssignmentDto
            {
                LessonId = 1,
                Content = "New Assignment",
                MeetLink = "http://meet.link"
            };

            var result = await repository.CreateAssignment(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Content, result.Content);
            Assert.Equal(dto.MeetLink, result.MeetLink);
        }

        [Fact]
        public async Task GetAssignment_ShouldReturnAssignment()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionAssignmentDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AssignmentRepository>();
            var repository = new AssignmentRepository(dao, _mapper, logger);

            var assignment = new Assignment
            {
                LessonId = 1,
                Content = "Sample Content",
                MeetLink = "http://test.com"
            };
            context.Assignment.Add(assignment);
            await context.SaveChangesAsync();

            var result = await repository.GetAssignment(assignment.AssignmentId);

            Assert.NotNull(result);
            Assert.Equal("Sample Content", result.Content);
        }

        [Fact]
        public async Task DeleteAssignment_ShouldRemoveAssignment()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionAssignmentDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AssignmentRepository>();
            var repository = new AssignmentRepository(dao, _mapper, logger);

            var assignment = new Assignment
            {
                LessonId = 2,
                Content = "To delete"
            };
            context.Assignment.Add(assignment);
            await context.SaveChangesAsync();

            var result = await repository.DeleteAssignment(assignment.AssignmentId);

            Assert.True(result);
            Assert.Null(await context.Assignment.FindAsync(assignment.AssignmentId));
        }

        [Fact]
        public async Task UpdateAssignment_ShouldUpdateSuccessfully()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionAssignmentDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AssignmentRepository>();
            var repository = new AssignmentRepository(dao, _mapper, logger);

            var assignment = new Assignment
            {
                LessonId = 1,
                Content = "Old",
                MeetLink = "old"
            };
            context.Assignment.Add(assignment);
            await context.SaveChangesAsync();

            var dto = new UpdateAssignmentDto
            {
                AssignmentId = assignment.AssignmentId,
                LessonId = assignment.LessonId,
                Content = "Updated",
                MeetLink = "new"
            };

            var result = await repository.UpdateAssignment(dto);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.Content);
            Assert.Equal("new", result.MeetLink);
        }

        [Fact]
        public async Task GetAllAssignmentByLessonId_ShouldReturnCorrectList()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionAssignmentDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AssignmentRepository>();
            var repository = new AssignmentRepository(dao, _mapper, logger);

            context.Assignment.AddRange(
                new Assignment { LessonId = 1, Content = "A1" },
                new Assignment { LessonId = 1, Content = "A2" },
                new Assignment { LessonId = 2, Content = "B1" }
            );
            await context.SaveChangesAsync();

            var result = await repository.GetAllAssignmentByLessonId(1);

            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal(1, r.LessonId));
        }
    }

}
