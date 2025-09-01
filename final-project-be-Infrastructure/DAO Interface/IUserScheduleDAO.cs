using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IUserScheduleDAO : IGenericDAO<UserSchedule>
    {
        public Task<List<UserSchedule>> GetSchedulesByUserIdAsync(Guid userId);
        public Task<List<UserSchedule>> GetByScheduleIdAsync(int scheduleId);
        public Task DeleteUserSchedulesByScheduleIdAsync(int scheduleId);
    }

}
