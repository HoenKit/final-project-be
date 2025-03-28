using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
    public class NotificationDAO : GenericDAO<Notification>
    {
        public NotificationDAO(ApplicationDbContext context) : base(context)
        {
        }
    }
}
