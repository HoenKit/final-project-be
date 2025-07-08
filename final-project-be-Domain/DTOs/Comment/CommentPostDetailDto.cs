using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Comment
{
    public class CommentPostDetailDto
    {
        public int CommentId { get; set; }
        public int PostId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar {  get; set; }
        public Guid UserId { get; set; }
        public int? ParentCommentId { get; set; }
        public string Content { get; set; }
    }
}
