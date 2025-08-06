using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class UserAssignmentDAO : GenericDAO<UserAssignment>, IUserAssignmentDAO
    {
        private readonly ApplicationDbContext _context;
        public UserAssignmentDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<UserAssignment>> GetUserAssignmentsByAssignmentIdAsync(int assignmentId)
        {
            return await _context.UserAssignments
                .Where(ua => ua.AssignmentId == assignmentId
                          && ua.IsScored == false
                          && ua.IsPresented == true)
                .Include(ua => ua.Assignment)
                .ToListAsync();
        }

        public async Task<UserAssignment?> GetUserAssignmentAsync(Guid userId, int assignmentId)
        {
            return await _context.UserAssignments
                .FirstOrDefaultAsync(x => x.UserId == userId && x.AssignmentId == assignmentId);
        }

        public async Task<List<UserAssignment>> GetUserAssignmentsByUserIdsAndAssignmentIdAsync(List<Guid> userIds, int assignmentId)
        {
            return await _context.UserAssignments
                .Where(ua => userIds.Contains(ua.UserId) && ua.AssignmentId == assignmentId)
                .ToListAsync();
        }
    }

}
