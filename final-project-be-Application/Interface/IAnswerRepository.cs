using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
	public interface IAnswerRepository : IRepository<Answer>
	{
		public Task<Answer> CreateAnswer(AnswerDto dto);
		public Task<bool> DeleteAnswer(int id);
		public Task<Answer> GetAnswer(int id);
		public Task<Answer> UpdateAnswer(UpdateAnswerDto dto);
		public Task<ICollection<UpdateAnswerDto>> GetAllAnswerByQuestionId(int questionId);
	}
}
