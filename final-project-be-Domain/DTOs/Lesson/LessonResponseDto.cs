using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Lesson
{
	public class LessonResponseDto
	{
        public int LessonId { get; set; }
        public int ModuleId { get; set; }
		public string Title { get; set; }
		public string? Description { get; set; }    
        public string? DocumentLink { get; set; }
        public string? VideoLink { get; set; }
        public bool Ispassed { get; set; }
    }
}
