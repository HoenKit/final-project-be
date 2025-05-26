using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class ModuleDAO : GenericDAO<Module>
	{
		public ModuleDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
