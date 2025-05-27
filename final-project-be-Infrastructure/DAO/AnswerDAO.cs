using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.Data;

namespace final_project_be_Infrastructure.DAO
{
	public class AnswerDAO : GenericDAO<Answer>
	{
		public AnswerDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
