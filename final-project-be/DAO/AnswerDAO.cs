using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class AnswerDAO : GenericDAO<Answer>
	{
		public AnswerDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
