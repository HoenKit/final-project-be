using System.ComponentModel.DataAnnotations.Schema;
using final_project_be_Domain.DTOs.Comment;

namespace final_project_be_Domain.DTOs.Post
{
    public class PostDto
    {
        public int PostId { get; set; }
        public Guid UserId { get; set; }
        public int? ParentPostId { get; set; }
        public int CategoryId { get; set; }
        public bool? IsDeleted { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public List<PostFileDto>? PostFiles { get; set; }
        public List<CommentDto>? Comments { get; set; }
    }

    //UpdateAsync CreatePost
    public class PostCreateDto
    {
        public int PostId { get; set; }
        public Guid UserId { get; set; }
        public int? ParentPostId { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<PostFileCreateDto>? PostFileCreate { get; set; }
    }
}
