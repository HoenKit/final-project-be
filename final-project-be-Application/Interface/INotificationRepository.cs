using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Notification;

namespace final_project_be_Application.Interface
{
    public interface INotificationRepository : IRepository<Notification>
    {
        public Task<Notification> CreateNotification(NotificationDto dto);
        public Task<bool> DeleteNotification(int id);
        public Task<Notification> GetNotification(int id);
        public Task<Notification> UpdateNotification(NotificationDto dto);
        public PageResult<Notification> GetAllNotifications(int page, int pageSize);
        public Task<ICollection<Notification>> GetNotificationsByUser(Guid userId);
    }
}
