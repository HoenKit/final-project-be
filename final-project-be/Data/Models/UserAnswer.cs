using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class UserAnswer
    {
        [Key]
        public int UserAnswerId { get; set; }
        [ForeignKey("UserLesson")]
        public int UserLessonId { get; set; }
        [ForeignKey("Answer")]
        public int AnswerId { get; set; }
        public Answer? Answer { get; set; }
        public UserLesson? UserLesson { get; set; }
    }
}
