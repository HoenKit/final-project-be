using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data;
using final_project_be.Data.Models;
using final_project_be.Dtos.Comment;
using final_project_be.Dtos;
using final_project_be.Dtos.User;
using final_project_be.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace final_project_be.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly UserDAO _userDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(UserDAO userDAO, IMapper mapper, ILogger<UserRepository> logger)
            : base(userDAO)
        {
            _userDAO = userDAO;
            _mapper = mapper;
            _logger = logger;

        }

        public async Task<User> ToggleIsBanned(Guid userId)
        {
            _userDAO.BeginTransaction();
            try
            {
                var user = _userDAO.GetById(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return null;
                }

                user.IsBanned = !user.IsBanned;
                user.UpdateAt = DateTime.Now;

                _userDAO.Update(user);
                _userDAO.CommitTransaction();

                _logger.LogInformation($"User {userId} banned status changed to {user.IsBanned}");

                return user;
            }
            catch (Exception ex)
            {
                _userDAO.RollbackTransaction();
                _logger.LogError($"Failed to toggle ban status for User {userId}: {ex.Message}");
                return null;
            }
        }

        public PageResult<User> GetAllUsers(int page, int pageSize)
        {
            try
            {
                var totalCount = _userDAO.GetAll().Count();
                var users = _userDAO.GetAll()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get users success");

                return new PageResult<User>(users, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting users");
                return new PageResult<User>(new List<User>(), 0, page, pageSize);
            }
        }

        public async Task<User> GetUserandUserMetadata(Guid userId)
        {
            try
            {
                _userDAO.BeginTransaction();
                var user = _userDAO.GetUserandUserMetadata(userId);
                _userDAO.CommitTransaction();

                _logger.LogInformation("Get user success");
                return user;
            }
            catch (Exception ex)
            {
                _userDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get user");
                return null;
            }

        }

        public async Task<User> UpdateUser(UserManagerDto dto)
        {
            try
            {
                _userDAO.BeginTransaction();
                var user = _mapper.Map<User>(dto);
                _userDAO.Update(user);
                _userDAO.CommitTransaction();

                _logger.LogInformation("Update user success");
                return user;
            }
            catch (Exception ex)
            {
                _userDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update user");
                return null;
            }
        }
        public async Task<User> UpdateUserProfileAsync(UserProfileDto dto)
        {
            try
            {
                var user = _userDAO.GetUserandUserMetadata(dto.UserId);
            if (user == null)
            {
                return null;
            }

            _mapper.Map(dto, user);

            if (user.UserMetaData == null)
            {
                user.UserMetaData = new UserMetadata();
            }

            _mapper.Map(dto, user.UserMetaData);

                _userDAO.Update(user);

                _userDAO.SaveChanges();

            _logger.LogInformation("User profile and metadata updated successfully for UserId: {UserId}", dto.UserId);

            return user;}
            catch (Exception ex)
            {
                _userDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update user");
                return null;
            }
        }
        public List<MonthlyStatDto> GetUserStatisticsByMonth()
        {
            var allUsers = _userDAO.GetAll()
                .Where(u => u.CreateAt != null)
                .ToList();

            var stats = allUsers
                .GroupBy(u => new { u.CreateAt.Year, u.CreateAt.Month })
                .Select(g => new MonthlyStatDto
                {
                    Month = $"{g.Key.Month:D2}/{g.Key.Year}",
                    Total = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            return stats;
        }
    }
}
