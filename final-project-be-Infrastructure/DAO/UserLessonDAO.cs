using final_project_be_Domain.Models;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
	public class UserLessonDAO : GenericDAO<UserLesson>
	{
		private readonly ApplicationDbContext _context;
		public UserLessonDAO(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}
        public async Task<bool> UserLessonExists(Guid userId, int lessonId)=> await _context.UserLessons.AnyAsync(ul => ul.UserId == userId && ul.LessonId == lessonId);
        public async Task AddUserLessonAsync(UserLesson userLesson)=> await _context.UserLessons.AddAsync(userLesson);

        
    }
}
