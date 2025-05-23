using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Title { get; set; }
        [ForeignKey("ParentCategory")]
        public int? ParentCategoryId { get; set; }
        public string Description { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        public Category? ParentCategory { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<Courses>? Courses { get; set; }
        public ICollection<Category>? Categories { get; set; }
    }
}
