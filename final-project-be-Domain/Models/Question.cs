using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace final_project_be_Domain.Models
{
	public class Question
    {
        [Key]
        public int QuestionId { get; set; }
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }
        public string Question_text { get; set; }
        public Lesson? Lesson { get; set; }
        [JsonIgnore]
        public ICollection<Answer>? Answers { get; set; }
        public string QuestionType { get; set; }
    }
}
