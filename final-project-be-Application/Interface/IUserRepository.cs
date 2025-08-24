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
        public PageResult<User> GetAllUsers(int page, int pageSize);
        public List<MonthlyStatDto> GetUserStatisticsByMonth();
        public Task<User> UpdateUserPoint(decimal point, Guid userId);
        public Task<string> GetUserProfileSummaryAsync(Guid userId);
        public Task<bool> UpdateMetadataAsync(Guid userId, UpdateUserMetadataDto dto);
        public Task<IEnumerable<UserCertificateDto>> GetCertificatesByUserIdAsync(Guid userId);
        public Task<string?> UpdateAvatarAsync(Guid userId, UpdateAvatarDto dto);
    }
}
