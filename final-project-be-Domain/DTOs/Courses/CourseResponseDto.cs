using final_project_be_Domain.DTOs.Assignment;
using final_project_be_Domain.DTOs.Mentor;
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
		public string? CourseName { get; set; }
		public string? CourseContent { get; set; }
        public string? Requirement { get; set; }
        public string? IntendedLearner { get; set; }
        public string? Language { get; set; }
        public string? Level { get; set; }
        public decimal Cost { get; set; } = 0;
        public string? SkillLearn { get; set; }
        public int? StudentCount { get; set; }
		public string? CoursesImage { get; set; }
		public double? CourseLength { get; set; }
		public bool isDeleted { get; set; }
        public string? Status { get; set; }
        public DateTime? UpdateAt { get; set; }
		public int CountModule { get; set; }
		public int CountLesson { get; set; }
		public MentorDto Mentor { get; set; }
		public List<AssignmentDto>? Assignments { get; set; }
	}
}
