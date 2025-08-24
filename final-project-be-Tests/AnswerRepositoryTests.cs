using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace final_project_be_Tests
{
    public class AnswerRepositoryTests
    {
        private readonly Mock<IAnswerDAO> _answerDaoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<AnswerRepository>> _loggerMock;
        private readonly AnswerRepository _repository;

        public AnswerRepositoryTests()
        {
            _answerDaoMock = new Mock<IAnswerDAO>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<AnswerRepository>>();
            _repository = new AnswerRepository(
                _answerDaoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateAnswer_ShouldCreateAndReturnAnswer()
        {
            // Arrange
            var dto = new AnswerDto { QuestionId = 1, Text = "A", Is_correct = true };
            var answer = new Answer { AnswerId = 1, QuestionId = 1, Text = "A", Is_correct = true };
            _mapperMock.Setup(m => m.Map<Answer>(dto)).Returns(answer);
            _answerDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _answerDaoMock.Setup(m => m.AddAsync(answer)).Returns(Task.CompletedTask);
            _answerDaoMock.Setup(m => m.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateAnswer(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(answer, result);
            _answerDaoMock.Verify(m => m.AddAsync(answer), Times.Once);
            _answerDaoMock.Verify(m => m.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAnswer_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            // Arrange
            var dto = new AnswerDto { QuestionId = 1, Text = "A", Is_correct = true };
            _mapperMock.Setup(m => m.Map<Answer>(dto)).Throws(new Exception("Mapping failed"));
            _answerDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _answerDaoMock.Setup(m => m.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateAnswer(dto);

            // Assert
            Assert.Null(result);
            _answerDaoMock.Verify(m => m.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAnswer_ShouldReturnTrue_WhenDeleteSucceeds()
        {
            // Arrange
            _answerDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _answerDaoMock.Setup(m => m.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            _answerDaoMock.Setup(m => m.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.DeleteAnswer(1);

            // Assert
            Assert.True(result);
            _answerDaoMock.Verify(m => m.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAnswer_ShouldReturnFalseAndRollback_WhenExceptionThrown()
        {
            // Arrange
            _answerDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _answerDaoMock.Setup(m => m.DeleteAsync(It.IsAny<int>())).Throws(new Exception("Delete failed"));
            _answerDaoMock.Setup(m => m.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.DeleteAnswer(1);

            // Assert
            Assert.False(result);
            _answerDaoMock.Verify(m => m.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllAnswerByQuestionId_ShouldReturnDtos_WhenSuccess()
        {
            // Arrange
            var questionId = 1;
            var answers = new List<Answer>
            {
                new Answer { AnswerId = 1, QuestionId = questionId, Text = "A", Is_correct = true }
            };
            var dtos = new List<UpdateAnswerDto>
            {
                new UpdateAnswerDto { AnswerId = 1, QuestionId = questionId, Text = "A", Is_correct = true }
            };
            _answerDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _answerDaoMock.Setup(m => m.GetAll()).Returns(answers.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<UpdateAnswerDto>>(It.IsAny<List<Answer>>())).Returns(dtos);
            _answerDaoMock.Setup(m => m.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.GetAllAnswerByQuestionId(questionId);

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result.First().AnswerId);
        }

        [Fact]
        public async Task GetAllAnswerByQuestionId_ShouldReturnEmptyList_WhenExceptionThrown()
        {
            // Arrange
            _answerDaoMock.Setup(m => m.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _answerDaoMock.Setup(m => m.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.GetAllAnswerByQuestionId(1);

            // Assert
            Assert.Empty(result);
            _answerDaoMock.Verify(m => m.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAnswer_ShouldReturnAnswer_WhenFound()
        {
            // Arrange
            var answer = new Answer { AnswerId = 1, QuestionId = 2, Text = "A", Is_correct = true };
            _answerDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _answerDaoMock.Setup(m => m.GetByIdAsync(1)).ReturnsAsync(answer);
            _answerDaoMock.Setup(m => m.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.GetAnswer(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.AnswerId);
        }

        [Fact]
        public async Task GetAnswer_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            // Arrange
            _answerDaoMock.Setup(m => m.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _answerDaoMock.Setup(m => m.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.GetAnswer(1);

            // Assert
            Assert.Null(result);
            _answerDaoMock.Verify(m => m.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAnswer_ShouldUpdateAndReturnAnswer_WhenFound()
        {
            // Arrange
            var dto = new UpdateAnswerDto { AnswerId = 1, QuestionId = 2, Text = "Updated", Is_correct = false };
            var answer = new Answer { AnswerId = 1, QuestionId = 2, Text = "Old", Is_correct = true };
            _answerDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _answerDaoMock.Setup(m => m.GetByIdAsync(dto.AnswerId)).ReturnsAsync(answer);
            _mapperMock.Setup(m => m.Map(dto, answer)).Verifiable();
            _answerDaoMock.Setup(m => m.UpdateAsync(answer)).Returns(Task.CompletedTask);
            _answerDaoMock.Setup(m => m.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.UpdateAnswer(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.AnswerId);
            _answerDaoMock.Verify(m => m.UpdateAsync(answer), Times.Once);
        }

        [Fact]
        public async Task UpdateAnswer_ShouldReturnNullAndRollback_WhenAnswerNotFound()
        {
            // Arrange
            var dto = new UpdateAnswerDto { AnswerId = 1 };
            _answerDaoMock.Setup(m => m.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _answerDaoMock.Setup(m => m.GetByIdAsync(dto.AnswerId)).ReturnsAsync((Answer)null);
            _answerDaoMock.Setup(m => m.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.UpdateAnswer(dto);

            // Assert
            Assert.Null(result);
            _answerDaoMock.Verify(m => m.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAnswer_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            // Arrange
            var dto = new UpdateAnswerDto { AnswerId = 1 };
            _answerDaoMock.Setup(m => m.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _answerDaoMock.Setup(m => m.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.UpdateAnswer(dto);

            // Assert
            Assert.Null(result);
            _answerDaoMock.Verify(m => m.RollbackTransactionAsync(), Times.Once);
        }
    }
}