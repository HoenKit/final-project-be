using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class LessonDAO : GenericDAO<Lesson>, ILessonDAO
    {
        private readonly ApplicationDbContext _context;

        public LessonDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> CountLessonsInModule(int moduleId)
            => await _context.Lessons.CountAsync(l => l.ModuleId == moduleId);

        public async Task<int> CountCompletedLessonsInModule(Guid userId, int moduleId)
            => await _context.UserLessons.CountAsync(ul => ul.UserId == userId && ul.Lesson.ModuleId == moduleId && ul.IsPassed);

        public async Task<List<int>> GetModuleIdsByCourseId(int courseId)
            => await _context.Modules.Where(m => m.CourseId == courseId).Select(m => m.ModuleId).ToListAsync();

        public async Task<List<UserAnswer>> GetUserAnswersWithDetailsAsync(int userLessonId)
            => await _context.UserAnswers
                .Where(ua => ua.UserLessonId == userLessonId)
                .Include(ua => ua.Answer)
                .ToListAsync();

        public async Task<Lesson?> GetLessonByIdAsync(int lessonId)
            => await _context.Lessons.FindAsync(lessonId);

        public async Task<List<Lesson>> GetLessonsByModuleId(int moduleId)
            => await _context.Lessons.Where(l => l.ModuleId == moduleId).ToListAsync();

        public async Task<bool> IsQuizLessonAsync(int lessonId)
            => await _context.Question.AnyAsync(q => q.LessonId == lessonId);

        public async Task<bool> HasQuestionAsync(int lessonId)
            => await _context.Question.AnyAsync(q => q.LessonId == lessonId);

        public async Task<bool> HasAssignmentAsync(int lessonId)
            => await _context.Assignment.AnyAsync(a => a.LessonId == lessonId);

        public async Task<List<UserLesson>> GetUserPassedLessons(Guid userId, List<int> lessonIds)
            => await _context.UserLessons
                .Where(ul => ul.UserId == userId && lessonIds.Contains(ul.LessonId) && ul.IsPassed)
                .ToListAsync();

        public async Task<UserLesson?> GetUserLessonAsync(Guid userId, int lessonId)
            => await _context.UserLessons.FirstOrDefaultAsync(ul => ul.UserId == userId && ul.LessonId == lessonId);

        public async Task AddUserAnswersAsync(List<UserAnswer> userAnswers)
            => await _context.UserAnswers.AddRangeAsync(userAnswers);
    }

}
