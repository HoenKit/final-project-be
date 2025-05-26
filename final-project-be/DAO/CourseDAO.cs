using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class CourseDAO : GenericDAO<Courses>
	{
		public CourseDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
