using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class UserAnswerDAO : GenericDAO<UserAnswer>
	{
		public UserAnswerDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
