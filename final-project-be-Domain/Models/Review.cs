using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        [ForeignKey("Courses")]
        public int CourseId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; } = false;
        public decimal Rate { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        public User? User { get; set; }
        public Courses? Courses { get; set; }
    }
}
