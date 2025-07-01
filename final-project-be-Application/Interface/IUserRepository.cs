using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.DTOs.Users;

namespace final_project_be_Application.Interface
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> ToggleIsBanned(Guid userId);
        public Task<User> GetUserandUserMetadata(Guid userId);
        public Task<User> UpdateUser(UserManagerDto dto);
        public PageResult<User> GetAllUsers(int page, int pageSize);
        public Task<User> UpdateUserProfileAsync(UserProfileDto dto);
        public List<MonthlyStatDto> GetUserStatisticsByMonth();
        public Task<User> UpdateUserPoint(decimal point, Guid userId);
        public Task<string> GetUserProfileSummaryAsync(Guid userId);
    }
}
