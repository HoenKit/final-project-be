using final_project_be_Domain.DTOs.Users;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IUserDAO : IGenericDAO<User>
    {
        Task<User> GetByIdAsync(Guid userId);
        bool UserRegisterExist(UserRegisterDto registerDto);
        User GetUserbyEmail(string email);
        IEnumerable<string> GetRolesByUserId(Guid userId);
        Task<Role> GetRoleByNameAsync(string roleName);
        Task<bool> ExistsAsync(Guid userId, int roleId);
        Task AddUserRoleAsync(UserRole userRole);
        Task AddRoleAsync(Role role);
        Task AddUserMetaData(UserMetadata userMetadata);
        Task<UserMetadata> GetUserMetadatabyId(Guid userId);
        Task<User> GetUserByMentor(int mentorId);
        Task UpdateUserMetadataAsync(UserMetadata user);
        Task CreateUserAsync(User user);
    }

}
