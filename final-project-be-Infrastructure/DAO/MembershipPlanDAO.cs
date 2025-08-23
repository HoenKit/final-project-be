using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using System;

namespace final_project_be_Infrastructure.DAO
{
    public class MembershipPlanDAO : GenericDAO<MembershipPlan>, IMembershipPlanDAO
    {
        private readonly ApplicationDbContext _context;
        public MembershipPlanDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<MembershipPlan?> GetPlanByIdAsync(int planId)
    => await _context.MembershipPlans.FindAsync(planId);

    }

}
