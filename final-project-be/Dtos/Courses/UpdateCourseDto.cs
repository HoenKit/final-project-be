namespace final_project_be.Dtos.Courses
{
	public class UpdateCourseDto
	{
		public int CourseId { get; set; }
		public int MentorId { get; set; }
		public int CategoryId { get; set; }
		public string CourseName { get; set; }
		public string CourseContent { get; set; }
		public string Cost { get; set; }
		public string SkillLearn { get; set; }
	}
}
