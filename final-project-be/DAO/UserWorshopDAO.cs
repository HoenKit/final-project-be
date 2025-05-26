using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class UserWorshopDAO : GenericDAO<UserWorkshop>
	{
		public UserWorshopDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
