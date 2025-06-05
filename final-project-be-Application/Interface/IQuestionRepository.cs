using final_project_be_Domain.DTOs.Question;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
	public interface IQuestionRepository : IRepository<Question>
	{
		public Task<Question> CreateQuestion(QuestionDto dto);
		public Task<bool> DeleteQuestion(int id);
		public Task<Question> GetQuestion(int id);
		public Task<Question> UpdateQuestion(UpdateQuestionDto dto);
		public Task<ICollection<UpdateQuestionDto>> GetAllQuestionByLessonId(int lessonId);
	}
}
