using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using Microsoft.EntityFrameworkCore;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Infrastructure.DAO
{
    public class CourseDAO : GenericDAO<Courses>, ICourseDAO
    {
        private readonly ApplicationDbContext _context;

        public CourseDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Courses?> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Mentor)
                    .ThenInclude(m => m.User)
                        .ThenInclude(u => u.UserMetaData)
                .Include(c => c.Modules)
                    .ThenInclude(m => m.Lessons)
                    .ThenInclude(l => l.Assignments)
                .FirstOrDefaultAsync(c => c.CourseId == id);
        }
    }

}
