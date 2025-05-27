using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
	public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }
        public int Question_text { get; set; }
        public Lesson? Lesson { get; set; }
        public ICollection<Answer>? Answers { get; set; }
    }
}
