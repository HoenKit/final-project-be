using final_project_be_Domain.DTOs.LearnDto;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class UserLessonDAO : GenericDAO<UserLesson>, IUserLessonDAO
    {
        private readonly ApplicationDbContext _context;

        public UserLessonDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> UserLessonExists(Guid userId, int lessonId)
            => await _context.UserLessons.AnyAsync(ul => ul.UserId == userId && ul.LessonId == lessonId);

        public async Task AddUserLessonAsync(UserLesson userLesson)
            => await _context.UserLessons.AddAsync(userLesson);

        public async Task<UserLesson?> GetUserLessonbyuserandlessonAsync(Guid userId, int lessonId)
            => await _context.UserLessons
                .FirstOrDefaultAsync(ul => ul.UserId == userId && ul.LessonId == lessonId);

        public async Task<List<UserLesson>> GetUserLessonsByModuleAsync(Guid userId, int moduleId)
            => await _context.UserLessons
                .Where(ul => ul.UserId == userId && ul.Lesson.ModuleId == moduleId)
                .Include(ul => ul.Lesson)
                .ToListAsync();

        public async Task DeleteUserLessonAsync(UserLesson entity)
        {
            _context.UserLessons.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public Task<List<UserLesson>> GetUserLessonsAsync(Guid userId)
            => _context.UserLessons
                .Where(ul => ul.UserId == userId)
                .ToListAsync();
    }

}
