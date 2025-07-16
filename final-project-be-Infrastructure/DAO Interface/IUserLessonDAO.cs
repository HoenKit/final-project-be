using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IUserLessonDAO : IGenericDAO<UserLesson>
    {
        Task<bool> UserLessonExists(Guid userId, int lessonId);
        Task AddUserLessonAsync(UserLesson userLesson);
        Task<UserLesson?> GetUserLessonbyuserandlessonAsync(Guid userId, int lessonId);
        Task<List<UserLesson>> GetUserLessonsByModuleAsync(Guid userId, int moduleId);
        Task DeleteUserLessonAsync(UserLesson entity);
        Task<List<UserLesson>> GetUserLessonsAsync(Guid userId);
    }

}
