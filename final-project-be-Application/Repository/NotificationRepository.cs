using AutoMapper;
using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Notification;
using final_project_be_Domain.DTOs;
using final_project_be_Application.Interface;
using Microsoft.Extensions.Logging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace final_project_be_Application.Repository
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly NotificationDAO _NotificationDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationRepository> _logger;

        public NotificationRepository(NotificationDAO NotificationDAO, IMapper mapper, ILogger<NotificationRepository> logger) : base(NotificationDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _NotificationDAO = NotificationDAO;
        }

        public async Task<Notification> CreateNotification(NotificationDto dto)
        {
            try
            {
                await _NotificationDAO.BeginTransactionAsync();
                var Notification = _mapper.Map<Notification>(dto);
                await _NotificationDAO.AddAsync(Notification);
                await _NotificationDAO.CommitTransactionAsync();

                _logger.LogInformation("AddAsync Notification success");
                return Notification;
            }
            catch (Exception ex)
            {
                await _NotificationDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding Notification");
                return null;
            }
        }

        public async Task<bool> DeleteNotification(int id)
        {
            try
            {
                await _NotificationDAO.BeginTransactionAsync();
                await _NotificationDAO.DeleteAsync(id);
                await _NotificationDAO.CommitTransactionAsync();

                _logger.LogInformation("DeleteAsync Notification success");
                return true;
            }
            catch (Exception ex)
            {
                await _NotificationDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when delete Notification");
                return false;
            }
        }

        public PageResult<Notification> GetAllNotifications(int page, int pageSize)
        {
            try
            {
                var totalCount = _NotificationDAO.GetAll().Count();
                var Notifications = _NotificationDAO.GetAll()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get Notifications success");

                return new PageResult<Notification>(Notifications, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting Notifications");
                return new PageResult<Notification>(new List<Notification>(), 0, page, pageSize);
            }
        }

        public async Task<Notification> GetNotification(int id)
        {
            try
            {
                await _NotificationDAO.BeginTransactionAsync();
                var Notification = await _NotificationDAO.GetByIdAsync(id);
                await _NotificationDAO.CommitTransactionAsync();

                _logger.LogInformation("Get Notification success");
                return Notification;
            }
            catch (Exception ex)
            {
                await _NotificationDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when get Notification");
                return null;
            }

        }

        public async Task<ICollection<Notification>> GetNotificationsByUser(Guid userId)
        {
            try
            {
                var notifications = _NotificationDAO.GetAll()
                    .Where(n => n.UserId == userId)
                    .Take(5)
                    .ToList();

                _logger.LogInformation("Get Notifications success");

                return await Task.FromResult(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting Notifications");
                return new List<Notification>();
            }
        }

        public async Task<Notification> UpdateNotification(NotificationDto dto)
        {
            try
            {
                await _NotificationDAO.BeginTransactionAsync();
                var Notification = _mapper.Map<Notification>(dto);
                await _NotificationDAO.UpdateAsync(Notification);
                await _NotificationDAO.CommitTransactionAsync();

                _logger.LogInformation("UpdateAsync Notification success");
                return Notification;
            }
            catch (Exception ex)
            {
                await _NotificationDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when UpdateAsync Notification");
                return null;
            }
        }
    }
}
