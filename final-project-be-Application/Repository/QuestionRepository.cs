using AutoMapper;
using Azure;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Bibliography;
using final_project_be_Application.Interface;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Question;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{
	public class QuestionRepository : Repository<Question>, IQuestionRepository
	{
		private readonly IQuestionDAO _questionDAO;
		private readonly IMapper _mapper;
		private readonly ILogger<QuestionRepository> _logger;
		private readonly IAnswerRepository _answerRepository;
		private readonly IOpenAIEmbeddingService _openAIService;
		public QuestionRepository(IQuestionDAO questionDAO, IMapper mapper, ILogger<QuestionRepository> logger, IAnswerRepository answerRepository, IOpenAIEmbeddingService openAIService) : base(questionDAO)
		{
			_questionDAO = questionDAO;
			_mapper = mapper;
			_logger = logger;
			_answerRepository = answerRepository;
			_openAIService = openAIService;
		}

		public async Task<Question> CreateQuestion(QuestionDto dto)
		{
			try
			{
				await _questionDAO.BeginTransactionAsync();
				var question = _mapper.Map<Question>(dto);
				var validTypes = new[] { "SingleChoice", "MultipleChoice" };
				if (!validTypes.Contains(question.QuestionType))
				{
					_logger.LogError($"Invalid QuestionType '{question.QuestionType}' for question: {question.Question_text}");
					throw new Exception($"Invalid QuestionType '{question.QuestionType}' for question: {question.Question_text}");
				}
				await _questionDAO.AddAsync(question);
				await _questionDAO.CommitTransactionAsync();
				_logger.LogInformation("AddAsync Question success");
				return question;
			}
			catch (Exception ex)
			{
				await _questionDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when adding Question");
				return null;
			}
		}

		public async Task<bool> DeleteQuestion(int id)
		{
			try
			{
				await _questionDAO.BeginTransactionAsync();
				await _questionDAO.DeleteAsync(id);
				await _questionDAO.CommitTransactionAsync();
				_logger.LogInformation("Delete Question success");
				return true;
			}
			catch (Exception ex)
			{
				await _questionDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when delete Question");
				return false;
			}
		}

		public async Task<ICollection<UpdateQuestionDto>> GetAllQuestionByLessonId(int lessonId)
		{
			try
			{
				await _questionDAO.BeginTransactionAsync();

				var questions = _questionDAO.GetAll()
					.Where(m => m.LessonId == lessonId)
					.ToList();

				var questionDtos = _mapper.Map<List<UpdateQuestionDto>>(questions);

				await _questionDAO.CommitTransactionAsync();

				_logger.LogInformation("Successfully retrieved questions for lesson ID {lessonId}", lessonId);
				return questionDtos;
			}
			catch (Exception ex)
			{
				await _questionDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error while retrieving questions for lesson ID {lessonId}", lessonId);
				return new List<UpdateQuestionDto>();
			}
		}

		public async Task<Question> GetQuestion(int id)
		{
			try
			{
				await _questionDAO.BeginTransactionAsync();
				var question = await _questionDAO.GetByIdAsync(id);
				await _questionDAO.CommitTransactionAsync();
				_logger.LogInformation("Get Question success");
				return question;
			}
			catch (Exception ex)
			{
				await _questionDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when getting Question");
				return null;
			}
		}

		public async Task<Question> UpdateQuestion(UpdateQuestionDto dto)
		{
			try
			{
				await _questionDAO.BeginTransactionAsync();
				var question = await _questionDAO.GetByIdAsync(dto.QuestionId);
				if (question == null)
				{
					_logger.LogWarning("Question not found with ID: {Id}", dto.QuestionId);
					await _questionDAO.RollbackTransactionAsync();
					return null;
				}
				_mapper.Map(dto, question);
				var validTypes = new[] { "SingleChoice", "MultipleChoice" };
				if (!validTypes.Contains(question.QuestionType))
				{
					_logger.LogError($"Invalid QuestionType '{question.QuestionType}' for question: {question.Question_text}");
					throw new Exception($"Invalid QuestionType '{question.QuestionType}' for question: {question.Question_text}");
				}
				await _questionDAO.UpdateAsync(question);
				await _questionDAO.CommitTransactionAsync();
				_logger.LogInformation("UpdateAsync Question success");
				return question;
			}
			catch (Exception ex)
			{
				await _questionDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when updating Question");
				return null;
			}
		}

		public async Task ImportQuestionsFromExcel(IFormFile file, int lessonId)
		{
			using var stream = new MemoryStream();
			await file.CopyToAsync(stream);
			using var workbook = new XLWorkbook(stream);
			var worksheet = workbook.Worksheets.First();
			var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header row

			// Group answers by QuestionText
			var grouped = rows
				.Select(r =>
				{
					string questionText = r.Cell(1).GetString().Trim();
					string answerText = r.Cell(2).GetString().Trim();
					string isCorrectStr = r.Cell(3).GetString().Trim();
					string questionType = r.Cell(4).GetString().Trim();

					bool isCorrect = bool.TryParse(isCorrectStr, out var result) && result;

					return new
					{
						QuestionText = questionText,
						AnswerText = answerText,
						IsCorrect = isCorrect,
						QuestionType = questionType
					};
				})
				.GroupBy(x => x.QuestionText);

			foreach (var group in grouped)
			{
				// Use first item in group to get the question type
				var first = group.First();

				var validTypes = new[] { "SingleChoice", "MultipleChoice" };
				if (!validTypes.Contains(first.QuestionType))
				{
					_logger.LogError($"Invalid QuestionType '{first.QuestionType}' for question: {first.QuestionText}");
					throw new Exception($"Invalid QuestionType '{first.QuestionType}' for question: {first.QuestionText}");
				}

				// Create Question DTO
				var questionDto = new QuestionDto
				{
					LessonId = lessonId,
					Question_text = first.QuestionText,
					QuestionType = first.QuestionType
				};

				// Save Question
				var question = await CreateQuestion(questionDto);

				// Save Answers
				foreach (var item in group)
				{
					var answerDto = new AnswerDto
					{
						QuestionId = question.QuestionId,
						Text = item.AnswerText,
						Is_correct = item.IsCorrect
					};

					await _answerRepository.CreateAnswer(answerDto);
				}
			}
		}

        public async Task<bool> ImportQuizFromAI(IFormFile pdfFile, int lessonId, int number, string difficulty)
        {
            try
            {
                var fileId = await _openAIService.UploadFileToOpenAIAsync(pdfFile);

                string rawResponse = await _openAIService.GenerateQuizFromPdfAsync(fileId, number, difficulty);

                var jsonMatch = Regex.Match(rawResponse, @"```json\s*(.+?)\s*```", RegexOptions.Singleline);
                string jsonToDeserialize = jsonMatch.Success ? jsonMatch.Groups[1].Value : rawResponse;

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var quizQuestions = JsonSerializer.Deserialize<List<QuizQuestion>>(jsonToDeserialize, options);

                if (quizQuestions == null || quizQuestions.Count == 0)
                {
                    _logger.LogWarning("AI response doesn't contain valid questions.");
                    return false;
                }

                foreach (var q in quizQuestions)
                {
                    var validTypes = new[] { "SingleChoice", "MultipleChoice" };
                    if (!validTypes.Contains(q.QuestionType)) continue;

                    var questionDto = new QuestionDto
                    {
                        LessonId = lessonId,
                        Question_text = q.QuestionText,
                        QuestionType = q.QuestionType
                    };

                    var question = await CreateQuestion(questionDto);
                    if (question == null) continue;

                    foreach (var ans in q.Answers)
                    {
                        var answerDto = new AnswerDto
                        {
                            QuestionId = question.QuestionId,
                            Text = ans.Text,
                            Is_correct = ans.IsCorrect
                        };

                        await _answerRepository.CreateAnswer(answerDto);
                    }
                }

                _logger.LogInformation("Import quiz from AI success.");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while importing quiz from AI");
                return false;
            }
        }

    }
}
