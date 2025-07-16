using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IUserCourseDAO : IGenericDAO<UserCourse>
    {
        Task<UserCourse> GetUserCourse(Guid userId, int courseId);
        Task<bool> UserCourseExists(Guid userId, int courseId);
        Task<List<UserCourse>> GetUserCoursesByUserId(Guid userId);
        Task UpdateCertificateLinkAsync(Guid userId, int courseId, string link);
        Task<UserCourse?> GetCompletedUserCourseAsync(Guid userId, int courseId);
        Task AddUserCourseAsync(UserCourse userCourse);
        Task UpdateUserCourse(UserCourse userCourse);
    }

}
