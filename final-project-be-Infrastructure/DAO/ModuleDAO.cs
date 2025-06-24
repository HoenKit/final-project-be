using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
	public class ModuleDAO : GenericDAO<Module>
	{
		private readonly ApplicationDbContext _context;
		public ModuleDAO(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}
		public async Task<Module?> GetByIdAsync(int id){
			return await _context.Modules.Include(c => c.Lessons).FirstOrDefaultAsync(c => c.ModuleId == id);
		}
        public async Task<List<Module>> GetModulesByCourseId(int courseId)=> await _context.Modules.Where(m => m.CourseId == courseId).ToListAsync();
        public async Task<Module?> GetByCourseIdAsync(int CourseId)
        {
            return await _context.Modules.Include(c => c.Lessons).FirstOrDefaultAsync(c => c.CourseId == CourseId);
        }
    }
}
