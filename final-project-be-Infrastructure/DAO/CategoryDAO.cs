using final_project_be_Domain.Models;
using final_project_be_Infrastructure.Data;
using final_project_be_Infrastructure.DAO;

namespace final_project_be_Infrastructure.DAO
{
	public class CategoryDAO : GenericDAO<Category>
	{
		public CategoryDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
