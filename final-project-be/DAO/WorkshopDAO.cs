using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class WorkshopDAO : GenericDAO<WorkShop>
	{
		public WorkshopDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
