using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class UserLesson
    {
        [Key]
        public int UserLessonId { get; set; }
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string Status { get; set; }
        public float? Percentage { get; set; }
        public DateTime? CompletedAt { get; set; }
        public float? Mark { get;set; }
        public ICollection<UserAnswer>? UserAnswer { get; set; }
        public User? User {  get; set; }
        public Lesson? Lesson { get; set; }
    }
}
