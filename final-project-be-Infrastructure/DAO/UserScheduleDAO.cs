using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class UserScheduleDAO : GenericDAO<UserSchedule>, IUserScheduleDAO
    {
        private readonly ApplicationDbContext _context;
        public UserScheduleDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<UserSchedule>> GetSchedulesByUserIdAsync(Guid userId)
        {
            return await _context.UserSchedules
                .Include(us => us.Schedule)
                    .ThenInclude(s => s.Courses)
                .Include(us => us.Schedule)
                    .ThenInclude(s => s.Mentor)
                .Where(us => us.UserId == userId)
                .ToListAsync();
        }
    }

}
