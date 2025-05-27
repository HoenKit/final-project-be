using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;

namespace final_project_be_Infrastructure.DAO
{
	public class ReviewDAO : GenericDAO<Review>
	{
		public ReviewDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
