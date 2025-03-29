using final_project_be.Data.Models;
using final_project_be.Dtos.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Dtos.Post
{
    public class PostManagerDto
    {
        public int PostId { get; set; }
        public Guid UserId { get; set; }
        public int? ParentPostId { get; set; }
        public int SubCategoryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        public int ReportCount { get; set; }
        public List<PostFileDto>? PostFiles { get; set; }
    }
}
