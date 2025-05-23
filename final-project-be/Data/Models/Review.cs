using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
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
        public bool IsDeleted { get; set; }
        public decimal rate { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public User? User { get; set; }
        public Courses? Courses { get; set; }
    }
}
