using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;

namespace final_project_be_Infrastructure.DAO
{
	public class MentorDAO : GenericDAO<Mentor>
	{
		public MentorDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
