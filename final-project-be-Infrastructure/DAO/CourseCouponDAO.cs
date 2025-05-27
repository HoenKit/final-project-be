using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;

namespace final_project_be_Infrastructure.DAO
{
	public class CourseCouponDAO : GenericDAO<CourseCoupon>
	{
		public CourseCouponDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
