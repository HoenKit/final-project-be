using final_project_be_Domain.DTOs.LearnDto;
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
        public async Task<List<UserAssignment>> ListUserAssignmentNotScoresAsync(int assignmentId)
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
            return await _context.UserAssignments.Include(a => a.Assignment)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.AssignmentId == assignmentId);
        }
        public async Task<List<UserAssignmentInfoDto>> ListAssignmentsNotPresentAsync(int assignmentId)
        {
            return await _context.UserAssignments
                .Where(ua => ua.AssignmentId == assignmentId && !ua.IsPresented)
                .Include(ua => ua.User)
                .ThenInclude(um=>um.UserMetaData)
                .Select(ua => new UserAssignmentInfoDto
                {
                    AssignmentId = ua.AssignmentId,
                    UserId = ua.UserId,
                    FirstName = ua.User.UserMetaData.FirstName,
                    LastName = ua.User.UserMetaData.LastName,
                    IsPresented = ua.IsPresented,
                    IsScored = ua.IsScored,
                    content = ua.Content,                   
                })
                .ToListAsync();
        }

        public async Task<List<UserAssignment>> GetUserAssignmentsByUserIdsAndAssignmentIdAsync(List<Guid> userIds, int assignmentId)
        {
            return await _context.UserAssignments
                .Where(ua => userIds.Contains(ua.UserId) && ua.AssignmentId == assignmentId)
                .ToListAsync();
        }
        public async Task<UserAssignment> CreateUserAssignmentAsync(CreateUserAssignmentDto dto)
        {
            var newEntity = new UserAssignment
            {
                UserId = dto.UserId,
                AssignmentId = dto.AssignmentId,
                IsPresented = false,
                IsScored = false,
                CreateAt = DateTime.UtcNow
            };

            _context.UserAssignments.Add(newEntity);
            await _context.SaveChangesAsync();

            return newEntity;
        }

        public async Task<UserAssignment> UpdateUserAssignmentAsync(UserAssignment existing)
        {
            existing.IsPresented = false;
            existing.IsScored = false;
            existing.CreateAt = DateTime.UtcNow;

            _context.UserAssignments.Update(existing);
            await _context.SaveChangesAsync();

            return existing;
        }

    }
}
