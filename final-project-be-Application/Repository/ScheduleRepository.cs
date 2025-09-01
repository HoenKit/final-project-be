using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Schedule;
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
    public class ScheduleRepository : Repository<Schedule>, IScheduleRepository
    {
        private readonly IScheduleDAO _scheduleDAO;
        private readonly IUserScheduleDAO _userScheduleDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<AnswerRepository> _logger;


        public ScheduleRepository(IScheduleDAO scheduleDAO, IMapper mapper, ILogger<AnswerRepository> logger, IUserScheduleDAO userScheduleDAO) : base(scheduleDAO)
        {
            _scheduleDAO = scheduleDAO;
            _userScheduleDAO = userScheduleDAO;
            _mapper = mapper;
            _logger = logger;
            _userScheduleDAO = userScheduleDAO;
        }

        public async Task<bool> CreateScheduleAsync(ScheduleDto dto)
        {

            var schedule = _mapper.Map<Schedule>(dto);

            await _scheduleDAO.AddScheduleAsync(schedule);
            await _scheduleDAO.SaveChangesAsync();
            return true;
        }



        public async Task<bool> RegisterUserToScheduleAsync(UserScheduleDto dto)
        {
            var schedule = await _scheduleDAO.GetByIdAsync(dto.ScheduleId);

            if (schedule == null) return false;

            // Kiểm tra user đã học course chưa
            var enrolled = await _scheduleDAO.HasUserEnrolledCourseAsync(schedule.CourseId, dto.UserId);

            if (!enrolled) return false;

            var alreadyRegistered = await _scheduleDAO.IsUserAlreadyRegisteredAsync(dto.UserId, dto.ScheduleId);

            if (alreadyRegistered) return false;

            await _userScheduleDAO.AddAsync(new UserSchedule
            {
                UserId = dto.UserId,
                ScheduleId = dto.ScheduleId
            });

            await _scheduleDAO.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserScheduleDto>> GetUserSchedulesAsync(Guid userId)
        {
            var schedules = await _userScheduleDAO.GetSchedulesByUserIdAsync(userId);
            return _mapper.Map<List<UserScheduleDto>>(schedules);
        }

        public async Task<List<ScheduleDto>> GetSchedulesByCourseAsync(int courseId)
        {
            var schedules = await _scheduleDAO.GetSchedulesByCourseIdAsync(courseId);
            return _mapper.Map<List<ScheduleDto>>(schedules);
        }

        public async Task<List<GetScheduleDto>> GetSchedulesByMentorAsync(int mentorId)
        {
            var schedules = await _scheduleDAO.GetSchedulesByMentorIdAsync(mentorId);

            var scheduleDtos = schedules.Select(s =>
            {
                var assignments = s.Courses.Modules
                    .SelectMany(m => m.Lessons)
                    .SelectMany(l => l.Assignments)
                    .OrderByDescending(a => a.CreateAt) 
                    .ToList();

                // Ưu tiên assignment có MeetLink
                var selectedAssignment = assignments.FirstOrDefault(a => !string.IsNullOrEmpty(a.MeetLink))
                                         ?? assignments.FirstOrDefault();

                return new GetScheduleDto
                {
                    ScheduleId = s.ScheduleId,
                    MentorId = s.MentorId,
                    ScheduleName = s.ScheduleName,
                    MentorDay = s.MentorDay,
                    CreateAt = s.CreateAt,
                    CourseId = s.CourseId,
                    MeetingLink = selectedAssignment?.MeetLink
                };
            }).ToList();

            return scheduleDtos;
        }

        public async Task<bool> DeleteScheduleAsync(int scheduleId)
        {
            try
            {
                await _userScheduleDAO.BeginTransactionAsync();

                // Xóa UserSchedules trước
                await _userScheduleDAO.DeleteUserSchedulesByScheduleIdAsync(scheduleId);

                // Xóa Schedule sau
                await _scheduleDAO.DeleteAsync(scheduleId);

                await _userScheduleDAO.CommitTransactionAsync();
                return true;
            }
            catch (Exception ex)
            {
                await _userScheduleDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error deleting Schedule {ScheduleId}", scheduleId);
                return false;
            }
        }

    }
}
