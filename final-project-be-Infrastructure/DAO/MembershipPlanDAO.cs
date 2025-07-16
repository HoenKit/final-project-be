using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Infrastructure.DAO
{
    public class MembershipPlanDAO : GenericDAO<MembershipPlan>, IMembershipPlanDAO
    {
        public MembershipPlanDAO(ApplicationDbContext context) : base(context)
        {
        }
    }

}
