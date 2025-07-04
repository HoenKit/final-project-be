using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace final_project_be_Domain.DTOs.Question
{
	public class QuestionDto
	{
		public int LessonId { get; set; }
		public string Question_text { get; set; }
		public string QuestionType { get; set; }
	}

    public class UploadExcelRequest
    {
        public IFormFile File { get; set; }

        public int LessonId { get; set; }
    }
}
