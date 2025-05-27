using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;

namespace final_project_be_Infrastructure.DAO
{
	public class CouponDAO : GenericDAO<Coupon>
	{
		public CouponDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
