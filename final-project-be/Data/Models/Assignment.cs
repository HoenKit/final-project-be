using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class Assignment
    {
        [Key]
        public int AssignmentId { get; set; }
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }
        public string Content { get; set; }
        public float Percentage { get; set; }
        public string MeetLink { get; set; }
        public DateTime CreateAt {  set; get; }
        public Lesson? Lesson { get; set; }
        public ICollection<UserAssignment>? UserAssignments { get; set; }
    }
}
