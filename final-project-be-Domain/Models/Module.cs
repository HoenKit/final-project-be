using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace final_project_be_Domain.Models
{
    public class Module
    {
        [Key]
        public int ModuleId { get; set; }
        [ForeignKey("Courses")]
        public int CourseId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsPremium { get; set; } = false;
        [JsonIgnore]
        public Courses? Courses { get; set; }
        [JsonIgnore]
        public ICollection<Lesson> Lessons { get; set; }
		[JsonIgnore]
		public ICollection<UserModule>? UserModules { get; set; }
    }
}
