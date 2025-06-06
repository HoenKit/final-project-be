using AutoMapper;
using Azure;
using ClosedXML.Excel;
using final_project_be_Application.Interface;
using final_project_be_Application.Service.AimlService;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Question;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
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
		private readonly QuestionDAO _questionDAO;
		private readonly IMapper _mapper;
		private readonly ILogger<QuestionRepository> _logger;
		private readonly IAnswerRepository _answerRepository;
		private readonly AimlService _aimlService;
		public QuestionRepository(QuestionDAO questionDAO, IMapper mapper, ILogger<QuestionRepository> logger, IAnswerRepository answerRepository, AimlService aimlService) : base(questionDAO)
		{
			_questionDAO = questionDAO;
			_mapper = mapper;
			_logger = logger;
			_answerRepository = answerRepository;
			_aimlService = aimlService;
		}

		public async Task<Question> CreateQuestion(QuestionDto dto)
		{
			try
			{
				await _questionDAO.BeginTransactionAsync();
				var question = _mapper.Map<Question>(dto);
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
			var rows = worksheet.RangeUsed().RowsUsed().Skip(1); // Skip header

			// Group by Question_text
			var grouped = rows
				.Select(r => new
				{
					QuestionText = r.Cell(1).GetString().Trim(),
					AnswerText = r.Cell(2).GetString().Trim(),
					IsCorrect = bool.TryParse(r.Cell(3).GetString().Trim(), out var result) ? result : false
				})
				.GroupBy(x => x.QuestionText);

			foreach (var group in grouped)
			{
				// Create Question
				var questionDto = new QuestionDto
				{
					LessonId = lessonId,
					Question_text = group.Key
				};

				var question = await CreateQuestion(questionDto);

				// Create Answers
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

		public async Task<bool> ImportQuizFromAI(string topic, int lessonId, int number)
		{
			var userPrompt = $@"
								Please generate exactly {number} multiple-choice questions on the topic {topic}.
Return ONLY a valid JSON array with the following structure (no explanation, no extra text,no markdown, no comments, no text before/after)
If the output is too long, return fewer questions but always a complete JSON.
Do not cut off or break JSON syntax:

[
  {{
    ""QuestionText"": ""..."",
    ""Answers"": [
      {{ ""Text"": ""..."", ""IsCorrect"": true }},
      {{ ""Text"": ""..."", ""IsCorrect"": false }},
      {{ ""Text"": ""..."", ""IsCorrect"": false }},
      {{ ""Text"": ""..."", ""IsCorrect"": false }}
    ]
  }}
]
";

			try
			{
				// Call API AI to get response JSON
				string rawResponse = await _aimlService.GetChatResponseAsync(userPrompt);

				// Nếu AI trả về kèm ```json ... ``` hoặc ```
				// Loại bỏ các dòng ```json, ``` và các phần không phải JSON
				var jsonMatch = Regex.Match(rawResponse, @"```json\s*(.+?)\s*```", RegexOptions.Singleline);

				string jsonToDeserialize;

				if (jsonMatch.Success)
				{
					jsonToDeserialize = jsonMatch.Groups[1].Value;
				}
				else
				{
					jsonToDeserialize = rawResponse;
				}

				// Nếu chuỗi jsonToDeserialize có escape thì bỏ escape:
				jsonToDeserialize = Regex.Unescape(jsonToDeserialize);

				// Deserialize
				var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
				var quizQuestions = JsonSerializer.Deserialize<List<QuizQuestion>>(jsonToDeserialize, options);


				if (quizQuestions == null || quizQuestions.Count == 0)
				{
					_logger.LogWarning("AI response don't have valid question.");
					return false;
				}

				foreach (var q in quizQuestions)
				{
					var questionDto = new QuestionDto
					{
						LessonId = lessonId,
						Question_text = q.QuestionText
					};

					var question = await CreateQuestion(questionDto);
					if (question == null)
					{
						_logger.LogWarning($"Can not create question: {q.QuestionText}");
						continue; 
					}

					foreach (var ans in q.Answers)
					{
						var answerDto = new AnswerDto
						{
							QuestionId = question.QuestionId, 
							Text = ans.Text,
							Is_correct = ans.IsCorrect
						};

						var answer = await _answerRepository.CreateAnswer(answerDto);
						if (answer == null)
						{
							_logger.LogWarning($"Can not create answer: {ans.Text} for question {q.QuestionText}");
						}
					}
				}

				_logger.LogInformation("Import quiz from AI success.");
				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error while import quiz from AI");
				return false;
			}
		}
	}
}
