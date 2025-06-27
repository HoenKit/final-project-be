using final_project_be_Domain.Models;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
	public class ScheduleDAO : GenericDAO<Schedule>
	{
		private readonly ApplicationDbContext _context;
		public ScheduleDAO(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}
        public async Task<Schedule> GetScheduleByIdAsync(int scheduleId)=> await _context.Schedules.FindAsync(scheduleId);
        public async Task<List<Schedule>> GetSchedulesByMentorIdAsync(int mentorId) => await _context.Schedules
                                                                                                    .Where(s => s.MentorId == mentorId)
                                                                                                    .ToListAsync();
        public async Task<bool> HasUserEnrolledCourseAsync(int courseId, Guid userId)
        {
            return await _context.Set<UserCourse>()
                .AnyAsync(cu => cu.CourseId == courseId && cu.UserId == userId);
        }
        public async Task<bool> IsUserAlreadyRegisteredAsync(Guid userId, int scheduleId)
        {
            return await _context.UserSchedules
                .AnyAsync(us => us.UserId == userId && us.ScheduleId == scheduleId);
        }

        public async Task<List<Schedule>> GetSchedulesByCourseIdAsync(int courseId)
        {
            return await _context.Schedules
                .Where(s => s.CourseId == courseId)
                .ToListAsync();
        }

        public async Task AddScheduleAsync(Schedule schedule)
        {
            _context.Schedules.Add(schedule);
            await _context.SaveChangesAsync();
        }

    }
}
