using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class UserCourse
    {
        [ForeignKey("Courses")]
        public int CourseId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set;}
        public string Status { get; set; }
        public string Percentage { get; set; }
        public DateTime CompletedAt { get; set; }
        public Courses? Courses { get; set; }
        public User? User { get; set; }
    }
}
