using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Lesson
{
	public class LessonDto
	{
		public int ModuleId { get; set; }
		public string Title { get; set; }
		public string? Description { get; set; }
        public IFormFile? Document { get; set; }
        public IFormFile? Video { get; set; }
	}
}
