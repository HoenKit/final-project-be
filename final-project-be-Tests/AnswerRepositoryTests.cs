using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.Data;
using final_project_be_Tests.TestDAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace final_project_be_Tests
{
    public class AnswerRepositoryTests
    {
        private readonly IMapper _mapper;

        public AnswerRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AnswerDto, Answer>();
                cfg.CreateMap<Answer, UpdateAnswerDto>();
                cfg.CreateMap<UpdateAnswerDto, Answer>();
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
        public async Task CreateAnswer_ShouldAddAnswer()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionAnswerDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AnswerRepository>();
            var repository = new AnswerRepository(dao, _mapper, logger);

            var dto = new AnswerDto
            {
                QuestionId = 1,
                Text = "Sample Answer",
                Is_correct = true
            };

            var result = await repository.CreateAnswer(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Text, result.Text);
            Assert.Equal(dto.Is_correct, result.Is_correct);
            Assert.Equal(dto.QuestionId, result.QuestionId);
        }

        [Fact]
        public async Task GetAnswer_ShouldReturnCorrectAnswer()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionAnswerDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AnswerRepository>();
            var repository = new AnswerRepository(dao, _mapper, logger);

            var answer = new Answer
            {
                QuestionId = 1,
                Text = "Test Answer",
                Is_correct = false
            };
            context.Answers.Add(answer);
            await context.SaveChangesAsync();

            var result = await repository.GetAnswer(answer.AnswerId);

            Assert.NotNull(result);
            Assert.Equal(answer.Text, result.Text);
        }

        [Fact]
        public async Task DeleteAnswer_ShouldRemoveAnswer()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionAnswerDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AnswerRepository>();
            var repository = new AnswerRepository(dao, _mapper, logger);

            var answer = new Answer
            {
                QuestionId = 1,
                Text = "To be deleted",
                Is_correct = true
            };
            context.Answers.Add(answer);
            await context.SaveChangesAsync();

            var result = await repository.DeleteAnswer(answer.AnswerId);

            Assert.True(result);
            Assert.Null(await context.Answers.FindAsync(answer.AnswerId));
        }

        [Fact]
        public async Task UpdateAnswer_ShouldModifyAnswer()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionAnswerDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AnswerRepository>();
            var repository = new AnswerRepository(dao, _mapper, logger);

            var answer = new Answer
            {
                QuestionId = 1,
                Text = "Old Text",
                Is_correct = false
            };
            context.Answers.Add(answer);
            await context.SaveChangesAsync();

            var dto = new UpdateAnswerDto
            {
                AnswerId = answer.AnswerId,
                QuestionId = 1,
                Text = "Updated Text",
                Is_correct = true
            };

            var updated = await repository.UpdateAnswer(dto);

            Assert.NotNull(updated);
            Assert.Equal("Updated Text", updated.Text);
            Assert.True(updated.Is_correct);
        }

        [Fact]
        public async Task GetAllAnswerByQuestionId_ShouldReturnAllMatchingAnswers()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionAnswerDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AnswerRepository>();
            var repository = new AnswerRepository(dao, _mapper, logger);

            context.Answers.AddRange(
                new Answer { QuestionId = 1, Text = "A1", Is_correct = false },
                new Answer { QuestionId = 1, Text = "A2", Is_correct = true },
                new Answer { QuestionId = 2, Text = "B1", Is_correct = false }
            );
            await context.SaveChangesAsync();

            var results = await repository.GetAllAnswerByQuestionId(1);

            Assert.Equal(2, results.Count);
            Assert.All(results, r => Assert.Equal(1, r.QuestionId));
        }
    }

}

