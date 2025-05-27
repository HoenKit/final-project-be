using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;

namespace final_project_be_Infrastructure.DAO
{
	public class WorkshopDAO : GenericDAO<WorkShop>
	{
		public WorkshopDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
