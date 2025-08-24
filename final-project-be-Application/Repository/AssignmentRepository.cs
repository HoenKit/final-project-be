using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Service.EmailService;
using final_project_be_Application.Service.GoogleMeetService;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Assignment;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
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
		private readonly IAssignmentDAO _assignmentDAO;
		private readonly IMapper _mapper;
		private readonly ILogger<AssignmentRepository> _logger;
        private readonly ClientSettings _clientSettings;
        private readonly IGoogleMeetService _googleMeetService;
        public AssignmentRepository(IAssignmentDAO assignmentDAO, IMapper mapper, IGoogleMeetService googleMeetService, ILogger<AssignmentRepository> logger, IOptions<ClientSettings> clientoptions) : base(assignmentDAO)
		{
			_assignmentDAO = assignmentDAO;
			_mapper = mapper;
			_clientSettings = clientoptions.Value;
            _logger = logger;
            _googleMeetService = googleMeetService;
        }

        public async Task<Assignment> CreateAssignment(AssignmentDto dto)
        {
            try
            {
                await _assignmentDAO.BeginTransactionAsync();

                var assignment = _mapper.Map<Assignment>(dto);

                // Tạo Google Meet link nếu không có link
                if (string.IsNullOrEmpty(assignment.MeetLink))
                {
                    // Tạo meeting link kéo dài 1 giờ từ thời điểm hiện tại
                    var startTime = DateTime.Now.AddHours(1);
                    var endTime = startTime.AddHours(1);

                    var meetTitle = $"Assignment Meeting - {assignment.LessonId}";
                    var meetDescription = assignment.Content;

                    var meetLink = await _googleMeetService.CreateGoogleMeetLinkAsync(
                        meetTitle,
                        startTime,
                        endTime,
                        meetDescription);

                    if (!string.IsNullOrEmpty(meetLink))
                    {
                        assignment.MeetLink = meetLink;
                    }
                }

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

        public async Task<List<GetAssignmentLessonDto>> GetAssignmentsBycreatorAsync(Guid userId)
        {
            var assignments = await _assignmentDAO.GetAssignmentsByUserIdAsync(userId);

            return assignments.Select(a => new GetAssignmentLessonDto
            {
                AssignmentId = a.AssignmentId,
                LessonId = a.LessonId,
                Content = a.Content,
                MeetLink = a.MeetLink,
				Title = a.Lesson?.Title
            }).ToList();
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
