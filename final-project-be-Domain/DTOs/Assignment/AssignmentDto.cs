using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Assignment
{
	public class AssignmentDto
	{
		public int LessonId { get; set; }
		public string Content { get; set; }
		public string? MeetLink { get; set; }
	}
}
