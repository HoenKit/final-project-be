using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class UserAssignmentDAO : GenericDAO<UserAssignment>
	{
		public UserAssignmentDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
