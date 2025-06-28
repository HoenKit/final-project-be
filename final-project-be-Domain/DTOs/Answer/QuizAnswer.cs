using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Answer
{
	public class QuizAnswer
	{
		public int AnswerId { get; set; }
		public string Text { get; set; }
		public bool IsCorrect { get; set; }
	}
}
