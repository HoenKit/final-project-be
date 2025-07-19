using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class WorkshopDAO : GenericDAO<WorkShop>, IWorkshopDAO
    {
        private readonly ApplicationDbContext _context;
        public WorkshopDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }


        public async Task<bool> MentorExists(int mentorId)
        {
            return await _context.Mentors.AnyAsync(m => m.MentorId == mentorId);
        }
    }

}
