using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class UserAssignment
    {
        [ForeignKey("Assignment")]
        public int AssignmentId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public DateTime? CreateAt { get; set; }
        public Assignment? Assignment { get; set; }
        public User? User { get; set; }
    }
}
