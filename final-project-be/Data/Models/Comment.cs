using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace final_project_be.Data.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        [ForeignKey("ParentComment")]
        public int? ParentCommentId { get; set; } 
        public string Content { get; set; } 
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        public Post? Post { get; set; }
        public User? User { get; set; }
        public Comment? ParentComment { get; set; }
        [JsonIgnore]
        public ICollection<Comment>? Comments { get; set; }
        [JsonIgnore]
        public ICollection<ReportComment>? ReportComments { get; set; }
    }
}
