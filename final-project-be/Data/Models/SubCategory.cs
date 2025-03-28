using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class SubCategory
    {
        [Key]
        public int SubCategoryId { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        public ICollection<Post>? Posts { get; set; }
        public Category? Category { get; set; }
    }
}
