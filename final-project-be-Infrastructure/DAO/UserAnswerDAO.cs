using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Infrastructure.DAO
{
    public class UserAnswerDAO : GenericDAO<UserAnswer>, IUserAnswerDAO
    {
        private readonly ApplicationDbContext _context;

        public UserAnswerDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task DeleteUserAnswersByUserLessonIdAsync(int userLessonId)
        {
            var answers = _context.UserAnswers
                .Where(ua => ua.UserLessonId == userLessonId);

            _context.UserAnswers.RemoveRange(answers);
            await _context.SaveChangesAsync();
        }
    }

}
