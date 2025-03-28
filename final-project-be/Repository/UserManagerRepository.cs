using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data;
using final_project_be.Data.Models;
using final_project_be.Dtos.Comment;
using final_project_be.Dtos;
using final_project_be.Dtos.User;
using final_project_be.Interface;
using Microsoft.Extensions.Logging;

namespace final_project_be.Repository
{
    public class UserManagerRepository : Repository<User>, IUserManagerRepository
    {
        private readonly UserManagerDAO _userManagerDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<UserManagerRepository> _logger;

        public UserManagerRepository(UserManagerDAO userManagerDAO, IMapper mapper, ILogger<UserManagerRepository> logger)
            : base(userManagerDAO)
        {
            _userManagerDAO = userManagerDAO;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<User> ToggleIsBanned(Guid userId)
        {
            _userManagerDAO.BeginTransaction();
            try
            {
                var user = _userManagerDAO.GetById(userId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {userId} not found.");
                    return null;
                }

                user.IsBanned = !user.IsBanned;
                user.UpdateAt = DateTime.Now;

                _userManagerDAO.Update(user);
                _userManagerDAO.CommitTransaction();

                _logger.LogInformation($"User {userId} banned status changed to {user.IsBanned}");

                return user;
            }
            catch (Exception ex)
            {
                _userManagerDAO.RollbackTransaction();
                _logger.LogError($"Failed to toggle ban status for User {userId}: {ex.Message}");
                return null;
            }
        }

        public PageResult<User> GetAllUsers(int page, int pageSize)
        {
            try
            {
                var totalCount = _userManagerDAO.GetAll().Count();
                var users = _userManagerDAO.GetAll()
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

        public async Task<User> GetUser(Guid userId)
        {
            try
            {
                _userManagerDAO.BeginTransaction();
                var user = _userManagerDAO.GetById(userId);
                _userManagerDAO.CommitTransaction();

                _logger.LogInformation("Get user success");
                return user;
            }
            catch (Exception ex)
            {
                _userManagerDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get user");
                return null;
            }

        }

        public async Task<User> UpdateUser(UserManagerDto dto)
        {
            try
            {
                _userManagerDAO.BeginTransaction();
                var user = _mapper.Map<User>(dto);
                _userManagerDAO.Update(user);
                _userManagerDAO.CommitTransaction();

                _logger.LogInformation("Update user success");
                return user;
            }
            catch (Exception ex)
            {
                _userManagerDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update user");
                return null;
            }
        }
    }
}
