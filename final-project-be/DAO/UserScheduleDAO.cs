using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class UserScheduleDAO : GenericDAO<UserSchedule>
	{
		public UserScheduleDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
