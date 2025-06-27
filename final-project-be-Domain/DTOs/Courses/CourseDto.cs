using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.DTOs.Courses
{
	public class CourseDto
	{
		public int MentorId { get; set; }
		public int CategoryId { get; set; }
		public string? CourseName { get; set; }
		public string? CourseContent { get; set; }
		public decimal Cost { get; set; } = 0;
		public string? SkillLearn { get; set; }
		public IFormFile? CoursesImage { get; set; }
		public double? CourseLength { get; set; }
	}
    public class UserCourseDto
    {
        public int CourseId { get; set; }
		public string? CourseName { get; set; }
		public string? CourseImage { get; set; }
        public string Status { get; set; } = "not started";
        public float? Percentage { get; set; }
        public string? CertificateLink { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
