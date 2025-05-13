using System.ComponentModel.DataAnnotations.Schema;
using final_project_be.Dtos.Comment;

namespace final_project_be.Dtos.Post
{
    public class PostDto
    {
        public int PostId { get; set; }
        public Guid UserId { get; set; }
        public int? ParentPostId { get; set; }
        public int SubCategoryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? CreateAt { get; set; }
        public List<PostFileDto>? PostFiles { get; set; }
        public List<CommentDto>? Comments { get; set; }
    }

    //Update CreatePost
    public class PostCreateDto
    {
        public int PostId { get; set; }
        public Guid UserId { get; set; }
        public int? ParentPostId { get; set; }
        public int SubCategoryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public List<PostFileCreateDto>? PostFileCreate { get; set; }
    }
}
