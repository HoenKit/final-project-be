using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.DTOs.Courses
{
	public class CourseDto
	{
		public int MentorId { get; set; }
		public int CategoryId { get; set; }
		public string CourseName { get; set; }
		public string CourseContent { get; set; }
		public decimal? Cost { get; set; }
		public string SkillLearn { get; set; }
	}
}
