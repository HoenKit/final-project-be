using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class Answer
    {
        [Key]
        public int AnswerId { get; set; }
        [ForeignKey("Question")]
        public int QuestionId { get; set;}
        public string Text { get; set; }
        public bool Is_correct { get; set; }
        public ICollection<UserAnswer>? UserAnswers { get; set; }
        public Question? Question { get; set; }
    }
}
