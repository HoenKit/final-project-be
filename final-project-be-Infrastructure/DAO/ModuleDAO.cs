using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using Microsoft.EntityFrameworkCore;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Infrastructure.DAO
{
    public class ModuleDAO : GenericDAO<Module>, IModuleDAO
    {
        private readonly ApplicationDbContext _context;

        public ModuleDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Module?> GetByIdAsync(int id)
        {
            return await _context.Modules
                .Include(m => m.Lessons)
                .FirstOrDefaultAsync(m => m.ModuleId == id);
        }

        public async Task<List<Module>> GetModulesByCourseId(int courseId)
        {
            return await _context.Modules
                .Where(m => m.CourseId == courseId)
                .ToListAsync();
        }

        public Task<List<Module>> GetModulesWithLessonsByCourseIdAsync(int courseId)
        {
            return _context.Modules
                .Where(m => m.CourseId == courseId)
                .Include(m => m.Lessons)
                .ToListAsync();
        }

        public async Task<Module?> GetByCourseIdAsync(int courseId)
        {
            return await _context.Modules
                .Include(m => m.Lessons)
                .FirstOrDefaultAsync(m => m.CourseId == courseId);
        }
    }

}
