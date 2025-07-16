using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IScheduleDAO : IGenericDAO<Schedule>
    {
        Task<Schedule> GetScheduleByIdAsync(int scheduleId);
        Task<List<Schedule>> GetSchedulesByMentorIdAsync(int mentorId);
        Task<bool> HasUserEnrolledCourseAsync(int courseId, Guid userId);
        Task<bool> IsUserAlreadyRegisteredAsync(Guid userId, int scheduleId);
        Task<List<Schedule>> GetSchedulesByCourseIdAsync(int courseId);
        Task AddScheduleAsync(Schedule schedule);
    }

}
