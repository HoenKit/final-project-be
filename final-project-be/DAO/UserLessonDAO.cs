using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class UserLessonDAO : GenericDAO<UserLesson>
	{
		public UserLessonDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
