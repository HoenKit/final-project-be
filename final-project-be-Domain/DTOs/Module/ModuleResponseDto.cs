using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Module
{
	public class ModuleResponseDto
	{
		public int ModuleId { get; set; }
		public int CourseId { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public bool IsPremium { get; set; }
		public int CountLesson { get; set; }
	}
}
