using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
	public class Assignment
	{
		[Key]
		public int AssignmentId { get; set; }
		[ForeignKey("Lesson")]
		public int LessonId { get; set; }
		public string Content { get; set; }
		public string? MeetLink { get; set; }
		public DateTime CreateAt { set; get; } = DateTime.Now;
		public Lesson? Lesson { get; set; }
		public ICollection<UserAssignment>? UserAssignments { get; set; }
	}
}
