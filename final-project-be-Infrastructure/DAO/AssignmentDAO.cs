using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class AssignmentDAO : GenericDAO<Assignment>, IAssignmentDAO
    {
        public ApplicationDbContext _context;
        public AssignmentDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Assignment>> GetAssignmentsByUserIdAsync(Guid userId)
        {
            return await _context.Assignment
                .Include(a => a.Lesson)
                    .ThenInclude(l => l.Module)
                    .ThenInclude(m => m.Courses)
                    .ThenInclude(u=> u.Mentor)
                    .ThenInclude(u => u.User)
                .Where(a => a.Lesson != null &&
                            a.Lesson.Module != null &&
                            a.Lesson.Module.Courses != null &&
                            a.Lesson.Module.Courses.Mentor.UserId == userId)
                .ToListAsync();
        }
    }

}
