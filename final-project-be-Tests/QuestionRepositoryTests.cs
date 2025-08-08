using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Question;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Tests
{
    public class QuestionRepositoryTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IQuestionDAO> _questionDAOMock = new();
        private readonly Mock<ILogger<QuestionRepository>> _loggerMock = new();
        private readonly Mock<IAnswerRepository> _answerRepositoryMock = new();
        private readonly Mock<IOpenAIEmbeddingService> _embeddingServiceMock = new();
        private readonly QuestionRepository _questionRepo;

        public QuestionRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<QuestionDto, Question>().ReverseMap();
                cfg.CreateMap<UpdateQuestionDto, Question>().ReverseMap();
                cfg.CreateMap<AnswerDto, AnswerDto>().ReverseMap();
            });

            _mapper = config.CreateMapper();

            _questionRepo = new QuestionRepository(
                _questionDAOMock.Object,
                _mapper,
                _loggerMock.Object,
                _answerRepositoryMock.Object,
                _embeddingServiceMock.Object
            );
        }
        [Fact]
        public async Task CreateQuestion_ShouldReturnQuestion_WhenValidType()
        {
            // Arrange
            var dto = new QuestionDto
            {
                Question_text = "2 + 2 = ?",
                QuestionType = "SingleChoice",
                LessonId = 10
            };

            _questionDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.AddAsync(It.IsAny<Question>())).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _questionRepo.CreateQuestion(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Question_text, result.Question_text);
            Assert.Equal(dto.QuestionType, result.QuestionType);
            Assert.Equal(dto.LessonId, result.LessonId);
        }

        [Fact]
        public async Task CreateQuestion_ShouldReturnNull_WhenInvalidType()
        {
            // Arrange
            var dto = new QuestionDto
            {
                Question_text = "Essay question?",
                QuestionType = "Essay", 
                LessonId = 5
            };

            _questionDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _questionRepo.CreateQuestion(dto);

            // Assert
            Assert.Null(result);
            _questionDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteQuestion_ShouldReturnTrue_WhenSuccess()
        {
            // Arrange
            int questionId = 1;

            _questionDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.DeleteAsync(questionId)).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _questionRepo.DeleteQuestion(questionId);

            // Assert
            Assert.True(result);
            _questionDAOMock.Verify(d => d.DeleteAsync(questionId), Times.Once);
            _questionDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteQuestion_ShouldReturnFalse_WhenExceptionThrown()
        {
            // Arrange
            int questionId = 2;

            _questionDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.DeleteAsync(questionId)).ThrowsAsync(new Exception("Delete failed"));
            _questionDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _questionRepo.DeleteQuestion(questionId);

            // Assert
            Assert.False(result);
            _questionDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAllQuestionByLessonId_ShouldReturnCorrectQuestions()
        {
            // Arrange
            var lessonId = 1;

            var questions = new List<Question>
    {
        new Question { QuestionId = 1, LessonId = lessonId, Question_text = "Q1", QuestionType = "SingleChoice" },
        new Question { QuestionId = 2, LessonId = lessonId, Question_text = "Q2", QuestionType = "MultipleChoice" },
        new Question { QuestionId = 3, LessonId = 2, Question_text = "Q3", QuestionType = "SingleChoice" }
    };

            var queryableQuestions = questions.AsQueryable();

            _questionDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.GetAll()).Returns(queryableQuestions);

            // Act
            var result = await _questionRepo.GetAllQuestionByLessonId(lessonId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, q => Assert.Equal(lessonId, q.LessonId));
        }

        [Fact]
        public async Task GetAllQuestionByLessonId_ShouldReturnEmptyList_WhenExceptionThrown()
        {
            // Arrange
            var lessonId = 1;

            _questionDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.GetAll()).Throws(new Exception("Database failure"));
            _questionDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _questionRepo.GetAllQuestionByLessonId(lessonId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _questionDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task GetQuestion_ShouldReturnQuestion_WhenExists()
        {
            // Arrange
            var questionId = 1;
            var expectedQuestion = new Question
            {
                QuestionId = questionId,
                LessonId = 1,
                Question_text = "What is .NET?",
                QuestionType = "SingleChoice"
            };

            _questionDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.GetByIdAsync(questionId)).ReturnsAsync(expectedQuestion);
            _questionDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _questionRepo.GetQuestion(questionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedQuestion.QuestionId, result.QuestionId);
            Assert.Equal(expectedQuestion.Question_text, result.Question_text);
            _questionDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _questionDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _questionDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task GetQuestion_ShouldReturnNull_WhenExceptionThrown()
        {
            // Arrange
            var questionId = 1;

            _questionDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.GetByIdAsync(questionId)).ThrowsAsync(new Exception("Database error"));
            _questionDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _questionRepo.GetQuestion(questionId);

            // Assert
            Assert.Null(result);
            _questionDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _questionDAOMock.Verify(d => d.GetByIdAsync(questionId), Times.Once);
            _questionDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Never);
            _questionDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateQuestion_ShouldReturnUpdatedQuestion_WhenValid()
        {
            // Arrange
            var dto = new UpdateQuestionDto
            {
                QuestionId = 1,
                LessonId = 10,
                Question_text = "Updated question?",
                QuestionType = "SingleChoice"
            };

            var existingQuestion = new Question
            {
                QuestionId = 1,
                LessonId = 5,
                Question_text = "Old question?",
                QuestionType = "MultipleChoice"
            };

            _questionDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.GetByIdAsync(dto.QuestionId)).ReturnsAsync(existingQuestion);
            _questionDAOMock.Setup(d => d.UpdateAsync(It.IsAny<Question>())).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _questionRepo.UpdateQuestion(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Question_text, result.Question_text);
            Assert.Equal(dto.QuestionType, result.QuestionType);
            _questionDAOMock.Verify(d => d.UpdateAsync(It.IsAny<Question>()), Times.Once);
            _questionDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateQuestion_ShouldReturnNull_WhenQuestionNotFound()
        {
            // Arrange
            var dto = new UpdateQuestionDto
            {
                QuestionId = 99,
                LessonId = 1,
                Question_text = "Invalid question?",
                QuestionType = "SingleChoice"
            };

            _questionDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.GetByIdAsync(dto.QuestionId)).ReturnsAsync((Question)null);
            _questionDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _questionRepo.UpdateQuestion(dto);

            // Assert
            Assert.Null(result);
            _questionDAOMock.Verify(d => d.UpdateAsync(It.IsAny<Question>()), Times.Never);
            _questionDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateQuestion_ShouldReturnNull_WhenQuestionTypeInvalid()
        {
            // Arrange
            var dto = new UpdateQuestionDto
            {
                QuestionId = 1,
                LessonId = 10,
                Question_text = "Invalid question type",
                QuestionType = "TextAnswer" 
            };

            var existingQuestion = new Question
            {
                QuestionId = 1,
                LessonId = 5,
                Question_text = "Some question",
                QuestionType = "SingleChoice"
            };

            _questionDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(d => d.GetByIdAsync(dto.QuestionId)).ReturnsAsync(existingQuestion);
            _questionDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _questionRepo.UpdateQuestion(dto);

            // Assert
            Assert.Null(result);
            _questionDAOMock.Verify(d => d.UpdateAsync(It.IsAny<Question>()), Times.Never);
            _questionDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task ImportQuestionsFromExcel_ShouldImportSuccessfully()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Arrange
            var excelStream = new MemoryStream();
            using (var package = new ExcelPackage(excelStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells[1, 1].Value = "QuestionText";
                worksheet.Cells[1, 2].Value = "AnswerText";
                worksheet.Cells[1, 3].Value = "IsCorrect";
                worksheet.Cells[1, 4].Value = "QuestionType";

                worksheet.Cells[2, 1].Value = "What is 2+2?";
                worksheet.Cells[2, 2].Value = "4";
                worksheet.Cells[2, 3].Value = "true";
                worksheet.Cells[2, 4].Value = "SingleChoice";

                package.Save();
            }
            excelStream.Position = 0;

            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Returns<Stream, CancellationToken>((stream, _) =>
                {
                    excelStream.CopyTo(stream);
                    return Task.CompletedTask;
                });
            formFileMock.Setup(f => f.FileName).Returns("questions.xlsx");
            formFileMock.Setup(f => f.Length).Returns(excelStream.Length);
            formFileMock.Setup(f => f.OpenReadStream()).Returns(excelStream);

            _questionDAOMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(x => x.CommitTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(x => x.RollbackTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(x => x.AddAsync(It.IsAny<Question>())).Returns(Task.CompletedTask);

            _answerRepositoryMock.Setup(x => x.CreateAnswer(It.IsAny<AnswerDto>())).ReturnsAsync(new Answer());


            // Act
            await _questionRepo.ImportQuestionsFromExcel(formFileMock.Object, lessonId: 1);

            // Assert
            _questionDAOMock.Verify(x => x.AddAsync(It.Is<Question>(q => q.Question_text == "What is 2+2?")), Times.Once);
            _answerRepositoryMock.Verify(x => x.CreateAnswer(It.Is<AnswerDto>(a => a.Text == "4" && a.Is_correct)), Times.Once);
        }

        [Fact]
        public async Task ImportQuestionsFromExcel_ShouldThrowException_OnInvalidQuestionType()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            // Arrange
            var excelStream = new MemoryStream();
            using (var package = new ExcelPackage(excelStream))
            {
                var worksheet = package.Workbook.Worksheets.Add("Sheet1");
                worksheet.Cells[1, 1].Value = "QuestionText";
                worksheet.Cells[1, 2].Value = "AnswerText";
                worksheet.Cells[1, 3].Value = "IsCorrect";
                worksheet.Cells[1, 4].Value = "QuestionType";

                worksheet.Cells[2, 1].Value = "Invalid type question";
                worksheet.Cells[2, 2].Value = "Yes";
                worksheet.Cells[2, 3].Value = "false";
                worksheet.Cells[2, 4].Value = "UnknownType";

                package.Save();
            }
            excelStream.Position = 0;

            var formFileMock = new Mock<IFormFile>();
            formFileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Returns<Stream, CancellationToken>((stream, _) =>
                {
                    excelStream.CopyTo(stream);
                    return Task.CompletedTask;
                });

            _questionDAOMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _questionDAOMock.Setup(x => x.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act + Assert
            await Assert.ThrowsAsync<Exception>(() =>
                _questionRepo.ImportQuestionsFromExcel(formFileMock.Object, lessonId: 1));
        }
        [Fact]
        public async Task ImportQuizFromAI_ShouldImportSuccessfully()
        {
            // Arrange
            var jsonResponse = @"
[
  {
    ""QuestionText"": ""What is the capital of France?"",
    ""QuestionType"": ""SingleChoice"",
    ""Answers"": [
      { ""Text"": ""Paris"", ""IsCorrect"": true },
      { ""Text"": ""London"", ""IsCorrect"": false },
      { ""Text"": ""Berlin"", ""IsCorrect"": false },
      { ""Text"": ""Madrid"", ""IsCorrect"": false }
    ]
  }
]";

            _embeddingServiceMock
                .Setup(s => s.GetChatCompletionAsync(It.IsAny<string>()))
                .ReturnsAsync(jsonResponse);

            _questionDAOMock
                .Setup(d => d.AddAsync(It.IsAny<Question>()))
                .Returns((Question q) => Task.FromResult(q));

            _answerRepositoryMock
                .Setup(a => a.CreateAnswer(It.IsAny<AnswerDto>()))
                .ReturnsAsync(new Answer());

            // Act
            var result = await _questionRepo.ImportQuizFromAI("Geography", lessonId: 123, number: 1);

            // Assert
            Assert.True(result);
            _embeddingServiceMock.Verify(s => s.GetChatCompletionAsync(It.IsAny<string>()), Times.Once);
            _questionDAOMock.Verify(q => q.AddAsync(It.IsAny<Question>()), Times.Once);
            _answerRepositoryMock.Verify(a => a.CreateAnswer(It.IsAny<AnswerDto>()), Times.Exactly(4));
        }

        [Fact]
        public async Task ImportQuizFromAI_ShouldReturnFalse_WhenJsonIsInvalid()
        {
            var invalidJson = @"{ ""invalid"": ""json"" ";

            _embeddingServiceMock
                .Setup(s => s.GetChatCompletionAsync(It.IsAny<string>()))
                .ReturnsAsync(invalidJson);

            // Act
            var result = await _questionRepo.ImportQuizFromAI("Geography", lessonId: 123, number: 1);

            // Assert
            Assert.False(result);
            _embeddingServiceMock.Verify(s => s.GetChatCompletionAsync(It.IsAny<string>()), Times.Once);
            _questionDAOMock.Verify(q => q.AddAsync(It.IsAny<Question>()), Times.Never);
            _answerRepositoryMock.Verify(a => a.CreateAnswer(It.IsAny<AnswerDto>()), Times.Never);
        }
        [Fact]
        public async Task ImportQuizFromAI_ShouldReturnFalse_WhenOpenAIServiceFails()
        {
            // Arrange
            _embeddingServiceMock
                .Setup(s => s.GetChatCompletionAsync(It.IsAny<string>()))
                .ThrowsAsync(new Exception("OpenAI error"));

            // Act
            var result = await _questionRepo.ImportQuizFromAI("Geography", lessonId: 123, number: 1);

            // Assert
            Assert.False(result);
            _embeddingServiceMock.Verify(s => s.GetChatCompletionAsync(It.IsAny<string>()), Times.Once);
            _questionDAOMock.Verify(q => q.AddAsync(It.IsAny<Question>()), Times.Never);
            _answerRepositoryMock.Verify(a => a.CreateAnswer(It.IsAny<AnswerDto>()), Times.Never);
        }

    }
}
