using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface IUserCourseRepository : IRepository<UserCourse>
    {
        public Task<UserCourse> CreateUserCourse(CommentDto dto);
        public Task<bool> DeleteUserCourse(int id);
        public Task<UserCourse> GetUserCourse(int id);
        public Task<UserCourse> UpdateUserCourse(CommentDto dto);
    }
}
