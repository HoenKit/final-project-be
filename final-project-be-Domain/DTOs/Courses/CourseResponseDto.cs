using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Courses
{
	public class CourseResponseDto
	{
		public int CourseId { get; set; }
		public int MentorId { get; set; }
		public int CategoryId { get; set; }
		public string CourseName { get; set; }
		public string CourseContent { get; set; }
		public decimal Cost { get; set; }
		public string SkillLearn { get; set; }
		public int StudentCount { get; set; }
		public string? CoursesImage { get; set; }
		public double? CourseLength { get; set; }
		public DateTime? UpdateAt { get; set; }
		public int CountModule { get; set; }
		public int CountLesson { get; set; }
	}
}
