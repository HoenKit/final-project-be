using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
    public class UserCourse
    {
        [ForeignKey("Courses")]
        public int CourseId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set;}
        public string Status { get; set; }
        public float? Percentage { get; set; }
        public string? CertificateLink { get; set; }
        public DateTime CompletedAt { get; set; }
        public Courses? Courses { get; set; }
        public User? User { get; set; }
    }
}
