using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class UserCourseDAO : GenericDAO<UserCourse>, IUserCourseDAO
    {
        private readonly ApplicationDbContext _context;

        public UserCourseDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<UserCourse> GetUserCourse(Guid userId, int courseId)
            => await _context.UserCourses.FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

        public async Task<bool> UserCourseExists(Guid userId, int courseId)
            => await _context.UserCourses.AnyAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

        public async Task<List<UserCourse>> GetUserCoursesByUserId(Guid userId)
        {
            return await _context.UserCourses
                .Include(uc => uc.Courses)
                .Where(uc => uc.UserId == userId)
                .ToListAsync();
        }

        public async Task UpdateCertificateLinkAsync(Guid userId, int courseId, string link)
        {
            var userCourse = await _context.UserCourses
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId);

            if (userCourse != null)
            {
                userCourse.CertificateLink = link;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<UserCourse?> GetCompletedUserCourseAsync(Guid userId, int courseId)
        {
            return await _context.UserCourses
                .FirstOrDefaultAsync(uc => uc.UserId == userId && uc.CourseId == courseId && uc.Status == "completed");
        }

        public async Task AddUserCourseAsync(UserCourse userCourse)
            => await _context.UserCourses.AddAsync(userCourse);

        public async Task UpdateUserCourse(UserCourse userCourse)
        {
            _context.UserCourses.Update(userCourse);
            await _context.SaveChangesAsync();
        }
    }

}
