using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class ReviewDAO : GenericDAO<Review>
	{
		public ReviewDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
