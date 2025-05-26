using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class MembershipPlanDAO : GenericDAO<MembershipPlan>
	{
		public MembershipPlanDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
