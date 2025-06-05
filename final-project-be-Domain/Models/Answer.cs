using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace final_project_be_Domain.Models
{
	public class Answer
	{
		[Key]
		public int AnswerId { get; set; }
		[ForeignKey("Question")]
		public int QuestionId { get; set; }
		public string Text { get; set; }
		public bool Is_correct { get; set; }
		[JsonIgnore]
		public ICollection<UserAnswer>? UserAnswers { get; set; }
		public Question? Question { get; set; }
	}
}
