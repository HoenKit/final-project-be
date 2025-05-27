using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;

namespace final_project_be_Infrastructure.DAO
{
	public class LessonDAO : GenericDAO<Lesson>
	{
		public LessonDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
