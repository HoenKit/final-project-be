using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq.Expressions;

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

        [Fact]
        public async Task CreateAnswer_ShouldAddAnswer()
        {
            // Arrange
            var mockDao = new Mock<IAnswerDAO>();
            var dto = new AnswerDto { QuestionId = 1, Text = "Sample Answer", Is_correct = true };
            var entity = new Answer { AnswerId = 1, QuestionId = 1, Text = "Sample Answer", Is_correct = true };

            mockDao.Setup(d => d.AddAsync(It.IsAny<Answer>())).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.SaveChangesAsync()).Returns(Task.CompletedTask);

            var logger = Mock.Of<ILogger<AnswerRepository>>();
            var repo = new AnswerRepository(mockDao.Object, _mapper, logger);

            // Act
            var result = await repo.CreateAnswer(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Text, result.Text);
        }

        [Fact]
        public async Task GetAnswer_ShouldReturnCorrectAnswer()
        {
            // Arrange
            var answer = new Answer { AnswerId = 1, QuestionId = 1, Text = "Test Answer", Is_correct = false };
            var mockDao = new Mock<IAnswerDAO>();
            mockDao.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(answer);

            var logger = Mock.Of<ILogger<AnswerRepository>>();
            var repo = new AnswerRepository(mockDao.Object, _mapper, logger);

            // Act
            var result = await repo.GetAnswer(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Answer", result.Text);
        }

        [Fact]
        public async Task DeleteAnswer_ShouldRemoveAnswer()
        {
            var answer = new Answer { AnswerId = 1 };
            var mockDao = new Mock<IAnswerDAO>();
            mockDao.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(answer);
            mockDao.Setup(d => d.DeleteAsync(1)).Returns(Task.CompletedTask);

            var logger = Mock.Of<ILogger<AnswerRepository>>();
            var repo = new AnswerRepository(mockDao.Object, _mapper, logger);

            var result = await repo.DeleteAnswer(1);

            Assert.True(result);
        }

        [Fact]
        public async Task UpdateAnswer_ShouldModifyAnswer()
        {
            var original = new Answer { AnswerId = 1, QuestionId = 1, Text = "Old Text", Is_correct = false };

            var dto = new UpdateAnswerDto
            {
                AnswerId = 1,
                QuestionId = 1,
                Text = "Updated Text",
                Is_correct = true
            };

            var mockDao = new Mock<IAnswerDAO>();
            mockDao.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(original);
            mockDao.Setup(d => d.UpdateAsync(It.IsAny<Answer>())).Returns(Task.CompletedTask);

            var logger = Mock.Of<ILogger<AnswerRepository>>();
            var repo = new AnswerRepository(mockDao.Object, _mapper, logger);

            var updated = await repo.UpdateAnswer(dto);

            Assert.NotNull(updated);
            Assert.Equal("Updated Text", updated.Text);
            Assert.True(updated.Is_correct);
        }

        [Fact]
        public async Task GetAllAnswerByQuestionId_ShouldReturnAllMatchingAnswers()
        {
            // Arrange
            var mockDao = new Mock<IAnswerDAO>();
            var logger = new Mock<ILogger<AnswerRepository>>();
            var repository = new AnswerRepository(mockDao.Object, _mapper, logger.Object);

            var answers = new List<Answer>
    {
        new Answer { AnswerId = 1, QuestionId = 1, Text = "A1", Is_correct = false },
        new Answer { AnswerId = 2, QuestionId = 1, Text = "A2", Is_correct = true },
        new Answer { AnswerId = 3, QuestionId = 2, Text = "B1", Is_correct = false }
    };

            mockDao.Setup(d => d.GetAll()).Returns(answers.AsQueryable());

            var results = await repository.GetAllAnswerByQuestionId(1);

            Assert.Equal(2, results.Count);
            Assert.All(results, r => Assert.Equal(1, r.QuestionId));
        }

    }


}

