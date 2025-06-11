using final_project_be_Domain.DTOs.Category;
using final_project_be_Domain.DTOs.Mentor;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Courses
{
	public class GetCourseDto
	{
		public int CourseId { get; set; }
		public string CourseName { get; set; }
		public string CourseContent { get; set; }
		public decimal Cost { get; set; }
		public string SkillLearn { get; set; }
		public int StudentCount { get; set; }
		public string? CoursesImage { get; set; }
		public double? CourseLength { get; set; }
        public DateTime? CreateAt { get; set; }
        public MentorDto? Mentor { get; set; }
	}
}
