using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.Comment;
using final_project_be.Dtos.User;

namespace final_project_be.Interface
{
    public interface IUserManagerRepository : IRepository<User>
    {
        public Task<User> ToggleIsBanned(Guid userId);
        public Task<User> GetUser(Guid userId);
        public Task<User> UpdateUser(UserManagerDto dto);
        public PageResult<User> GetAllUsers(int page, int pageSize);
        public Task<User> UpdateUserProfileAsync(UserProfileDto dto);
    }
}
