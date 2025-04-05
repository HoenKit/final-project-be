using final_project_be.Data;
using final_project_be.Data.Models;
using final_project_be.Dtos.User;
using Microsoft.EntityFrameworkCore;

namespace final_project_be.DAO
{
    public class UserManagerDAO : GenericDAO<User>
    {
        private readonly ApplicationDbContext _context;
        public UserManagerDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
		public User GetById(Guid userId)
		{
			return _context.users
				.Include(u => u.UserMetaData) // Include UserMetadata
				.FirstOrDefault(u => u.UserId == userId);
		}
		public bool UserRegisterExist(UserRegisterDto registerDto) => _context.users.Any(u => u.Email == registerDto.Email);
        public User GetUserbyEmail(UserLoginDto loginDto) => _context.users.FirstOrDefault(u=> u.Email == loginDto.Email);
        public IEnumerable<string> GetRolesByUserId(Guid userId) => _context.userRoles.Where(ur => ur.UserId == userId).Select(ur => ur.Role.RoleName).ToList();
        public  Role GetRoleByName(string roleName)  => _context.roles.FirstOrDefault(r => r.RoleName == roleName);
        public  void AddUserRole(UserRole userRole) => _context.userRoles.Add(userRole);
        public  void AddRole(Role role) => _context.roles.Add(role);
        public void AddUserMetaData(UserMetadata userMetadata) => _context.UserMetadata.Add(userMetadata);
        public UserMetadata GetUserMetadatabyId(Guid UserId) => _context.UserMetadata.FirstOrDefault(u => u.UserId == UserId);
        public User GetUserandUserMetadata(Guid UserId) => _context.users.Include(u => u.UserMetaData).FirstOrDefault(u => u.UserId == UserId);
    }
}
