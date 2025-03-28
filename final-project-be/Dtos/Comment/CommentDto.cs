using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace final_project_be.Dtos.Comment
{
    public class CommentDto
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public Guid UserId { get; set; }
        public int? ParentCommentId { get; set; }
        public string Content { get; set; }
    }
}
