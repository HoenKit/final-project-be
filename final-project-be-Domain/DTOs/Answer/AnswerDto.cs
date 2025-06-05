using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Answer
{
	public class AnswerDto
	{
		public int QuestionId { get; set; }
		public string Text { get; set; }
		public bool Is_correct { get; set; }
	}
}
