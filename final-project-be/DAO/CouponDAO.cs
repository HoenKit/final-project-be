using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class CouponDAO : GenericDAO<Coupon>
	{
		public CouponDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
