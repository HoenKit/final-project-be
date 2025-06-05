using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Question;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{
	public class QuestionRepository : Repository<Question>, IQuestionRepository
	{
		private readonly QuestionDAO _questionDAO;
		private readonly IMapper _mapper;
		private readonly ILogger<QuestionRepository> _logger;
		public QuestionRepository(QuestionDAO questionDAO, IMapper mapper, ILogger<QuestionRepository> logger) : base(questionDAO)
		{
			_questionDAO = questionDAO;
			_mapper = mapper;
			_logger = logger;
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
	}
}
