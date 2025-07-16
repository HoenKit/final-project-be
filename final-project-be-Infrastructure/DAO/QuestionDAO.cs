using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using Microsoft.EntityFrameworkCore;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Infrastructure.DAO
{
    public class QuestionDAO : GenericDAO<Question>, IQuestionDAO
    {
        private readonly ApplicationDbContext _context;

        public QuestionDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Question>> GetQuestionsByLessonIdAsync(int lessonId)
            => await _context.Question
                             .Where(q => q.LessonId == lessonId)
                             .Include(q => q.Answers)
                             .ToListAsync();
    }

}
