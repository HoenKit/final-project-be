using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos.Notification;
using final_project_be.Dtos;
using final_project_be.Interface;

namespace final_project_be.Repository
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
                _NotificationDAO.BeginTransaction();
                var Notification = _mapper.Map<Notification>(dto);
                _NotificationDAO.Add(Notification);
                _NotificationDAO.CommitTransaction();

                _logger.LogInformation("Add Notification success");
                return Notification;
            }
            catch (Exception ex)
            {
                _NotificationDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when adding Notification");
                return null;
            }
        }

        public bool DeleteNotification(int id)
        {
            try
            {
                _NotificationDAO.BeginTransaction();
                _NotificationDAO.Delete(id);
                _NotificationDAO.CommitTransaction();

                _logger.LogInformation("Delete Notification success");
                return true;
            }
            catch (Exception ex)
            {
                _NotificationDAO.RollbackTransaction();
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
                _NotificationDAO.BeginTransaction();
                var Notification = _NotificationDAO.GetById(id);
                _NotificationDAO.CommitTransaction();

                _logger.LogInformation("Get Notification success");
                return Notification;
            }
            catch (Exception ex)
            {
                _NotificationDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get Notification");
                return null;
            }

        }

        public async Task<Notification> UpdateNotification(NotificationDto dto)
        {
            try
            {
                _NotificationDAO.BeginTransaction();
                var Notification = _mapper.Map<Notification>(dto);
                _NotificationDAO.Update(Notification);
                _NotificationDAO.CommitTransaction();

                _logger.LogInformation("Update Notification success");
                return Notification;
            }
            catch (Exception ex)
            {
                _NotificationDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update Notification");
                return null;
            }
        }
    }
}
