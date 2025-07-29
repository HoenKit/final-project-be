using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Users;
using Microsoft.EntityFrameworkCore;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Infrastructure.DAO
{
    public class UserDAO : GenericDAO<User>, IUserDAO
    {
        private readonly ApplicationDbContext _context;

        public UserDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(Guid userId)
            => await _context.users
                .Include(u => u.UserMetaData)
                .FirstOrDefaultAsync(u => u.UserId == userId);

        public bool UserRegisterExist(UserRegisterDto registerDto)
            => _context.users.Any(u => u.Email == registerDto.Email);

        public User GetUserbyEmail(string email)
            => _context.users.FirstOrDefault(u => u.Email == email);

        public IEnumerable<string> GetRolesByUserId(Guid userId)
            => _context.userRoles
                .Where(ur => ur.UserId == userId)
                .Select(ur => ur.Role.RoleName)
                .ToList();

        public async Task<Role> GetRoleByNameAsync(string roleName)
            => await _context.roles.FirstOrDefaultAsync(r => r.RoleName == roleName);

        public async Task<bool> ExistsAsync(Guid userId, int roleId)
            => await _context.userRoles.AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

        public async Task AddUserRoleAsync(UserRole userRole)
        {
            await _context.userRoles.AddAsync(userRole);
            await _context.SaveChangesAsync();
        }

        public async Task AddRoleAsync(Role role)
            => await _context.roles.AddAsync(role);

        public async Task AddUserMetaData(UserMetadata userMetadata)
            => await _context.UserMetadata.AddAsync(userMetadata);

        public async Task<UserMetadata> GetUserMetadatabyId(Guid userId)
            => await _context.UserMetadata.Include(u => u.User).FirstOrDefaultAsync(u => u.UserId == userId);

        public async Task<User> GetUserByMentor(int mentorId)
            => await _context.users.FirstOrDefaultAsync(u => u.Mentor.MentorId == mentorId);

        public async Task UpdateUserMetadataAsync(UserMetadata user)
        {
            _context.UserMetadata.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            _context.users.Add(user);
            await _context.SaveChangesAsync();
        }
    }

}
