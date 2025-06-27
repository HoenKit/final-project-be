using final_project_be_Domain.DTOs.Schedule;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface IScheduleRepository : IRepository<Schedule>
    {
        public Task<bool> CreateScheduleAsync(ScheduleDto dto);
        public Task<bool> RegisterUserToScheduleAsync(UserScheduleDto dto);
        public Task<List<ScheduleDto>> GetSchedulesByCourseAsync(int courseId);
    }
}
