using final_project_be_Domain.Models;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
	public class UserCourseDAO : GenericDAO<UserCourse>
	{
        private readonly ApplicationDbContext _context;
		public UserCourseDAO(ApplicationDbContext context) : base(context)
		{
            _context = context;
		}
        public async Task<UserCourse> GetUserCourse(Guid userId, int courseId )=>await _context.UserCourses.FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
        public async Task<bool> UserCourseExists(Guid userId, int courseId)=> await _context.UserCourses.AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseId);
        
        public async Task AddUserCourseAsync(UserCourse userCourse)=> await _context.UserCourses.AddAsync(userCourse);

        public async Task UpdateUserCourse(UserCourse userCourse)
         {
            _context.UserCourses.Update(userCourse);
            await _context.SaveChangesAsync();
         }

    }
}
