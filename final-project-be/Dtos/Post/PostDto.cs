using System.ComponentModel.DataAnnotations.Schema;

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
    }
}
