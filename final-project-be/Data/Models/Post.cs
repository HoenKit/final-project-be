using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        [ForeignKey("ParentPost")]
        public int? ParentPostId { get; set; }
        [ForeignKey("SubCategory")]
        public int SubCategoryId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        public SubCategory? SubCategory { get; set; }
        public User? User { get; set; }
        public Post? ParentPost { get; set; }
        public ICollection<PostFile>? PostFiles { get; set; }
        public ICollection<PollOption>? PollOptions { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<ReportPost>? ReportPosts { get; set; }
    }
}
