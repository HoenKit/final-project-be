using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class CourseCouponDAO : GenericDAO<CourseCoupon>
	{
		public CourseCouponDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
