using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;

namespace final_project_be_Infrastructure.DAO
{
	public class UserLessonDAO : GenericDAO<UserLesson>
	{
		public UserLessonDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
