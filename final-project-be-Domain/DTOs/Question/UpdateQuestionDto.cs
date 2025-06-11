using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Question
{
	public class UpdateQuestionDto
	{
		public int QuestionId { get; set; }
		public int LessonId { get; set; }
		public string Question_text { get; set; }
		public string QuestionType { get; set; }
	}
}
