using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Service.EmailService;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Assignment;
using final_project_be_Domain.DTOs.Assignment;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace final_project_be_Application.Repository
{
	public class AssignmentRepository : Repository<Assignment>, IAssignmentRepository
	{
		private readonly AssignmentDAO _assignmentDAO;
		private readonly IMapper _mapper;
		private readonly ILogger<AssignmentRepository> _logger;
        private readonly ClientSettings _clientSettings;
        public AssignmentRepository(AssignmentDAO assignmentDAO, IMapper mapper, ILogger<AssignmentRepository> logger, IOptions<ClientSettings> clientoptions) : base(assignmentDAO)
		{
			_assignmentDAO = assignmentDAO;
			_mapper = mapper;
			_clientSettings = clientoptions.Value;
            _logger = logger;
		}

		public async Task<Assignment> CreateAssignment(AssignmentDto dto)
		{
			try
			{
				await _assignmentDAO.BeginTransactionAsync();
				var assignment = _mapper.Map<Assignment>(dto);
				await _assignmentDAO.AddAsync(assignment);
				await _assignmentDAO.CommitTransactionAsync();
				_logger.LogInformation("AddAsync Assignment success");
				return assignment;
			}
			catch (Exception ex)
			{
				await _assignmentDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when adding Assignment");
				return null;
			}
		}

		public async Task<bool> DeleteAssignment(int id)
		{
			try
			{
				await _assignmentDAO.BeginTransactionAsync();
				await _assignmentDAO.DeleteAsync(id);
				await _assignmentDAO.CommitTransactionAsync();
				_logger.LogInformation("Delete Assignment success");
				return true;
			}
			catch (Exception ex)
			{
				await _assignmentDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when delete Assignment");
				return false;
			}
		}

		public async Task<ICollection<AssignmentResponseDto>> GetAllAssignmentByLessonId(int lessonId)
		{
			try
			{
				await _assignmentDAO.BeginTransactionAsync();

				var assignments = _assignmentDAO.GetAll()
					.Where(m => m.LessonId == lessonId)
					.ToList();

				var assignmentDtos = _mapper.Map<List<AssignmentResponseDto>>(assignments);

				await _assignmentDAO.CommitTransactionAsync();

				_logger.LogInformation("Successfully retrieved assignments for lesson ID {lessonId}", lessonId);
				return assignmentDtos;
			}
			catch (Exception ex)
			{
				await _assignmentDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error while retrieving assignments for lesson ID {lessonId}", lessonId);
				return new List<AssignmentResponseDto>();
			}
		}

		public async Task<Assignment> GetAssignment(int id)
		{
			try
			{
				await _assignmentDAO.BeginTransactionAsync();
				var assignment = await _assignmentDAO.GetByIdAsync(id);
				await _assignmentDAO.CommitTransactionAsync();
				_logger.LogInformation("Get Assignment success");
				return assignment;
			}
			catch (Exception ex)
			{
				await _assignmentDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when getting Assignment");
				return null;
			}
		}



        public async Task<Assignment> UpdateAssignment(UpdateAssignmentDto dto)
		{
			try
			{
				await _assignmentDAO.BeginTransactionAsync();
				var assignment = await _assignmentDAO.GetByIdAsync(dto.AssignmentId);
				if (assignment == null)
				{
					_logger.LogWarning("Assignment not found with ID: {Id}", dto.AssignmentId);
					await _assignmentDAO.RollbackTransactionAsync();
					return null;
				}
				_mapper.Map(dto, assignment);
				await _assignmentDAO.UpdateAsync(assignment);
				await _assignmentDAO.CommitTransactionAsync();
				_logger.LogInformation("UpdateAsync Assignment success");
				return assignment;
			}
			catch (Exception ex)
			{
				await _assignmentDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when updating Assignment");
				return null;
			}
		}
	}
}
