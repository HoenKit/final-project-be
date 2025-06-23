using final_project_be_Domain.Models;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
	public class LessonDAO : GenericDAO<Lesson>
    {
        private readonly ApplicationDbContext _context;
        public LessonDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public int CountLessonsInModule(int moduleId) => _context.Lessons.Count(l => l.ModuleId == moduleId);
        public int CountCompletedLessonsInModule(Guid userId, int moduleId) => _context.UserLessons.Count(ul => ul.UserId == userId && ul.Lesson.ModuleId == moduleId && ul.IsPassed);
        public List<int> GetModuleIdsByCourseId(int courseId) => _context.Modules.Where(m => m.CourseId == courseId).Select(m => m.ModuleId).ToList();
        public async Task<List<UserAnswer>> GetUserAnswersWithDetailsAsync(int userLessonId) => await _context.UserAnswers
                .Where(ua => ua.UserLessonId == userLessonId)
                .Include(ua => ua.Answer)
                .ToListAsync();
        public async Task<UserLesson?> GetUserLessonByIdAsync(int userLessonId) => await _context.UserLessons.FindAsync(userLessonId);
        public async Task<bool> IsQuizLessonAsync(int lessonId)=> await _context.Question.AnyAsync(q => q.LessonId == lessonId);
        

        public async Task<UserLesson?> GetUserLessonAsync(Guid userId, int lessonId)=> await _context.UserLessons.FirstOrDefaultAsync(ul => ul.UserId == userId && ul.LessonId == lessonId);
       
    }
}
