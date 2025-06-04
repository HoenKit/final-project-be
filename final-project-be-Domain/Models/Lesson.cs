using final_project_be_Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
	public class Lesson
    {
        [Key]
        public int LessonId { get; set; }
        [ForeignKey("Module")]
        public int ModuleId { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? VideoLink { get; set; }
        public ICollection<UserLesson>? UserLesson { get; set; }
        public ICollection<Assignment>? Assignments { get; set; }
        public ICollection<Question>? Questions { get; set; }
        public Module? Module { get; set; }
    }
}
