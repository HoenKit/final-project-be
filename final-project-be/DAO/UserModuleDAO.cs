using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class UserModuleDAO : GenericDAO<UserModule>
	{
		public UserModuleDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
