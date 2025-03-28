using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace final_project_be.Data.Models
{
    public class Report
    {
        [Key]
        public int ReportId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; } 
        public string Content { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        public User? User { get; set; }
        [JsonIgnore]
        public ICollection<ReportUser>? ReportUsers { get; set; }
        [JsonIgnore]
        public ICollection<ReportPost>? ReportPosts { get; set; }
        [JsonIgnore]
        public ICollection<ReportComment>? ReportComments { get; set; } 
    }
}
