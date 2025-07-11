using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
	public class CourseDAO : GenericDAO<Courses>
	{
		private readonly ApplicationDbContext _context;
		public CourseDAO(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}
		public async Task<Courses?> GetByIdAsync(int id)
		{
			return await _context.Courses.Include(c => c.Mentor).Include(c => c.Modules).ThenInclude(m => m.Lessons).FirstOrDefaultAsync(c => c.CourseId == id);
		}
	}
}
