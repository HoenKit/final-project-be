using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.DTOs.Users;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace final_project_be_Application.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly IUserDAO _userDAO;
        private readonly IMapper _mapper;
        private readonly IUserCourseDAO _userCourseDAO;
        private readonly ILogger<UserRepository> _logger;
        private readonly IBlobStorageService _blobStorageService;

        public UserRepository(IUserDAO userDAO, IMapper mapper, ILogger<UserRepository> logger, IUserCourseDAO userCourseDAO, IBlobStorageService blobStorageService)
            : base(userDAO)
        {
            _userDAO = userDAO;
            _mapper = mapper;
            _logger = logger;
            _userCourseDAO = userCourseDAO;
            _blobStorageService = blobStorageService;
        }
        public async Task<IEnumerable<UserCertificateDto>> GetCertificatesByUserIdAsync(Guid userId)
        {
            var userCourses = await _userCourseDAO.GetCertificatesByUserIdAsync(userId);

            return userCourses.Select(uc => new UserCertificateDto
            {
                UserId = uc.UserId,
                CourseId = uc.CourseId,
                MentorName = uc.Courses?.Mentor?.User?.UserMetaData?.FirstName + " "+ uc.Courses?.Mentor?.User?.UserMetaData?.LastName ?? "Unknown Mentor",
                Level = uc.Courses?.Level,
                CourseName = uc.Courses?.CourseName ?? "Unknown Course",
                CertificateLink = uc.CertificateLink,
                CompletedAt = uc.CompletedAt
            }).ToList();
        }
        public async Task<User> ToggleIsBanned(Guid userId)
        {
			await _userDAO.BeginTransactionAsync();
            try
            {
                var user = await _userDAO.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return null;
                }

                user.IsBanned = !user.IsBanned;
                user.UpdateAt = DateTime.Now;

                await _userDAO.UpdateAsync(user);
				await _userDAO.CommitTransactionAsync();

                _logger.LogInformation($"User {userId} banned status changed to {user.IsBanned}");

                return user;
            }
            catch (Exception ex)
            {
                await _userDAO.RollbackTransactionAsync();
                _logger.LogError($"Failed to toggle ban status for User {userId}: {ex.Message}");
                return null;
            }
        }

        public async Task<string?> UpdateAvatarAsync(Guid userId, UpdateAvatarDto dto)
        {
            try
            {
                await _userDAO.BeginTransactionAsync();

                var user = await _userDAO.GetByIdAsync(userId);
                if (user == null)
                {
                    await _userDAO.RollbackTransactionAsync();
                    return null;
                }

                if (dto.Avatar != null && dto.Avatar.Length > 0)
                {
                    // Lấy extension
                    var fileExtension = Path.GetExtension(dto.Avatar.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    // Upload lên Azure Blob Storage
                    using (var stream = dto.Avatar.OpenReadStream())
                    {
                        await _blobStorageService.UploadFileAsync(uniqueFileName, stream);
                    }

                    // Tạo URL public
                    var avatarUrl = $"https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/{uniqueFileName}";

                    // Update vào DB
                    user.UserMetaData.Avatar = avatarUrl;
                    await _userDAO.UpdateAsync(user);

                    await _userDAO.CommitTransactionAsync();
                    _logger.LogInformation("Update Avatar success for UserId {UserId}", userId);

                    return avatarUrl;
                }

                await _userDAO.RollbackTransactionAsync();
                throw new ArgumentException("Avatar file is required.");
            }
            catch (Exception ex)
            {
                await _userDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when updating avatar for UserId {UserId}", userId);
                return null;
            }
        }

        public PageResult<User> GetAllUsers(int page, int pageSize)
        {
            try
            {

                /*var totalCount = _userDAO.GetAll().Count();
                var users = _userDAO.GetAll()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();*/

                var query = _userDAO.GetAll().Include(u => u.UserMetaData);
                var totalCount = query.Count();
                var users = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get users success");

                return new PageResult<User>(users, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting users with metadata");
                return new PageResult<User>(new List<User>(), 0, page, pageSize);
            }
        }

        public async Task<User> GetUserandUserMetadata(Guid userId)
        {
            try
            {
                await _userDAO.BeginTransactionAsync();
                var user = await _userDAO.GetByIdAsync(userId);
				await _userDAO.CommitTransactionAsync();

                _logger.LogInformation("Get user success");
                return user;
            }
            catch (Exception ex)
            {
                await _userDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when get user");
                return null;
            }

        }

        public List<MonthlyStatDto> GetUserStatisticsByMonth()
        {
            try
            {
                var allUsers = _userDAO.GetAll()
                    .Where(u => u.CreateAt != null)
                    .ToList();

                _logger.LogInformation($"Processing {allUsers.Count} users for monthly statistics.");

                var stats = allUsers
                    .GroupBy(u => new { u.CreateAt.Year, u.CreateAt.Month })
                    .Select(g => new MonthlyStatDto
                    {
                        Month = $"{g.Key.Month:D2}/{g.Key.Year}",
                        Total = g.Count()
                    })
                    .OrderBy(x => x.Month)
                    .ToList();

                _logger.LogInformation("Successfully calculated monthly user statistics.");
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while calculating user statistics by month.");
                return new List<MonthlyStatDto>();
            }
        }

        public async Task<User> UpdateUserPoint(decimal point, Guid userId)
        {
            try
            {
                await _userDAO.BeginTransactionAsync();

                var user = await _userDAO.GetByIdAsync(userId);
                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", userId);
                    await _userDAO.RollbackTransactionAsync();
                    return null;
                }

                user.Point += point;

                await _userDAO.UpdateAsync(user);
                await _userDAO.CommitTransactionAsync();

                _logger.LogInformation("Update user points success");
                return user;
            }
            catch (Exception ex)
            {
                await _userDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when updating user points");
                return null;
            }
        }

        public async Task<bool> UpdateMetadataAsync(Guid userId, UpdateUserMetadataDto dto)
        {
            var metadata = await _userDAO.GetUserMetadatabyId(userId);
            if (metadata == null) return false;

            // Gán lại các field
            metadata.FirstName = dto.FirstName ?? metadata.FirstName;
            metadata.LastName = dto.LastName ?? metadata.LastName;
            metadata.Birthday = dto.Birthday ?? metadata.Birthday;
            metadata.Gender = dto.Gender ?? metadata.Gender;
            metadata.Avatar = dto.Avatar ?? metadata.Avatar;
            metadata.Address = dto.Address ?? metadata.Address;
            metadata.Nationality = dto.Nationality ?? metadata.Nationality;
            metadata.Level = dto.Level ?? metadata.Level;
            metadata.Goals = dto.Goals ?? metadata.Goals;
            metadata.FavouriteSubject = dto.FavouriteSubject ?? metadata.FavouriteSubject;

            await _userDAO.UpdateUserMetadataAsync(metadata);
            return true;
        }

        public async Task<string> GetUserProfileSummaryAsync(Guid userId)
        {
            var user = _userDAO.GetAll()
                .Include(u => u.UserMetaData)
                .FirstOrDefault(u => u.UserId == userId);

            if (user == null || user.UserMetaData == null)
                return "Unknown user with no metadata";

            var meta = user.UserMetaData;

            var parts = new List<string>();

            // Age
            if (meta.Birthday.HasValue)
            {
                var age = DateTime.Today.Year - meta.Birthday.Value.Year;
                if (meta.Birthday.Value > DateTime.Today.AddYears(-age)) age--;
                parts.Add($"{age}-year-old");
            }

            // Nationality
            if (!string.IsNullOrEmpty(meta.Nationality))
                parts.Add(meta.Nationality);

            // Level
            if (!string.IsNullOrEmpty(meta.Level))
                parts.Add(meta.Level.ToLower());

            // Goals
            if (!string.IsNullOrEmpty(meta.Goals))
                parts.Add($"who wants to {meta.Goals.ToLower()}");

            // Favourite Subject
            if (!string.IsNullOrEmpty(meta.FavouriteSubject))
                parts.Add($"and loves {meta.FavouriteSubject}");

            var summary = string.Join(" ", parts);

            return string.IsNullOrWhiteSpace(summary)
                ? "User with unspecified profile"
                : summary;
        }

    }
}
