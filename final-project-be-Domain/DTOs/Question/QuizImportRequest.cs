using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Question
{
	public class QuizImportRequest
	{
		public string Topic { get; set; }
		public int LessonId { get; set; }
		public int Number {  get; set; }
	}
}
