using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface IEventRepository : IRepository<Event>
    {
      public  Task<(bool Success, string Message, User? User)> AddPointsAsync(Guid userId, int points);
        public Task<(bool Success, string Message, User? User)> DailyLoginAsync(Guid userId);
    }
}
