using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class EventDAO : GenericDAO<Event>
	{
		public EventDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
