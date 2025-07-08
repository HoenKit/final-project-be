using final_project_be_Domain.DTOs.Comment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Post
{
    public class PostDetailDto
    {
        public int PostId { get; set; }
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Avatar { get; set; }
        public int? ParentPostId { get; set; }
        public string CategoryName { get; set; }
        public bool? IsDeleted { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public List<PostFileDto>? PostFiles { get; set; }
        public List<CommentPostDetailDto>? Comments { get; set; }
    }
}
