using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class QuestionDAO : GenericDAO<Question>
    {
        private readonly ApplicationDbContext _context;
        public QuestionDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Question>> GetQuestionsByLessonIdAsync(int lessonId)=> await _context.Question
                                                                                                    .Where(q => q.LessonId == lessonId)
                                                                                                    .Include(q => q.Answers)
                                                                                                    .ToListAsync();
        

    }
}
