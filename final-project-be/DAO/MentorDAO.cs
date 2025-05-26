using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class MentorDAO : GenericDAO<Mentor>
	{
		public MentorDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
