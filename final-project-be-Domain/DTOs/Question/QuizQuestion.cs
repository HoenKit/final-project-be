using final_project_be_Domain.DTOs.Answer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Question
{
	public class QuizQuestion
	{
		public string QuestionText { get; set; }
		public string QuestionType { get; set; }
		public List<QuizAnswer> Answers { get; set; }
	}
}
