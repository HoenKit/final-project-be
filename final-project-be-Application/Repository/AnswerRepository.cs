using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{
	public class AnswerRepository : Repository<Answer>, IAnswerRepository
	{
		private readonly IAnswerDAO _answerDAO;
		private readonly IMapper _mapper;
		private readonly ILogger<AnswerRepository> _logger;
		public AnswerRepository(IAnswerDAO answerDAO, IMapper mapper, ILogger<AnswerRepository> logger) : base(answerDAO)
		{
			_answerDAO = answerDAO;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<Answer> CreateAnswer(AnswerDto dto)
		{
			try
			{
				await _answerDAO.BeginTransactionAsync();
				var answer = _mapper.Map<Answer>(dto);
				await _answerDAO.AddAsync(answer);
				await _answerDAO.CommitTransactionAsync();
				_logger.LogInformation("AddAsync Answer success");
				return answer;
			}
			catch (Exception ex)
			{
				await _answerDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when adding Answer");
				return null;
			}
		}

		public async Task<bool> DeleteAnswer(int id)
		{
			try
			{
				await _answerDAO.BeginTransactionAsync();
				await _answerDAO.DeleteAsync(id);
				await _answerDAO.CommitTransactionAsync();
				_logger.LogInformation("Delete Answer success");
				return true;
			}
			catch (Exception ex)
			{
				await _answerDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when delete Answer");
				return false;
			}
		}

		public async Task<ICollection<UpdateAnswerDto>> GetAllAnswerByQuestionId(int questionId)
		{
			try
			{
				await _answerDAO.BeginTransactionAsync();

				var answers = _answerDAO.GetAll()
					.Where(m => m.QuestionId == questionId)
					.ToList();

				var answerDtos = _mapper.Map<List<UpdateAnswerDto>>(answers);

				await _answerDAO.CommitTransactionAsync();

				_logger.LogInformation("Successfully retrieved answers for question ID {questionId}", questionId);
				return answerDtos;
			}
			catch (Exception ex)
			{
				await _answerDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error while retrieving answers for question ID {questionId}", questionId);
				return new List<UpdateAnswerDto>();
			}
		}

		public async Task<Answer> GetAnswer(int id)
		{
			try
			{
				await _answerDAO.BeginTransactionAsync();
				var answer = await _answerDAO.GetByIdAsync(id);
				await _answerDAO.CommitTransactionAsync();
				_logger.LogInformation("Get Answer success");
				return answer;
			}
			catch (Exception ex)
			{
				await _answerDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when getting Answer");
				return null;
			}
		}

		public async Task<Answer> UpdateAnswer(UpdateAnswerDto dto)
		{
			try
			{
				await _answerDAO.BeginTransactionAsync();
				var answer = await _answerDAO.GetByIdAsync(dto.AnswerId);
				if (answer == null)
				{
					_logger.LogWarning("Answer not found with ID: {Id}", dto.AnswerId);
					await _answerDAO.RollbackTransactionAsync();
					return null;
				}
				_mapper.Map(dto, answer);
				await _answerDAO.UpdateAsync(answer);
				await _answerDAO.CommitTransactionAsync();
				_logger.LogInformation("UpdateAsync Answer success");
				return answer;
			}
			catch (Exception ex)
			{
				await _answerDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when updating Answer");
				return null;
			}
		}
	}
}
