using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.Notification;

namespace final_project_be.Interface
{
    public interface INotificationRepository : IRepository<Notification>
    {
        public Task<Notification> CreateNotification(NotificationDto dto);
        public bool DeleteNotification(int id);
        public Task<Notification> GetNotification(int id);
        public Task<Notification> UpdateNotification(NotificationDto dto);
        public PageResult<Notification> GetAllNotifications(int page, int pageSize);
    }
}
