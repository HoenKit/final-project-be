using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class WorkShop
    {
        [Key]
        public int WorkShopId { get; set; }
        [ForeignKey("Mentor")]
        public int MentorId { get; set; }
        public string Decription { get; set; }
        public string StreamingLink { get; set; }
        public DateTime CreateAt {  get; set; }
        public DateTime UpdateAt { get; set; }
        public Mentor? Mentor { get; set; }
        public ICollection<UserWorkshop>? UserWorkshops { get; set; }
        public ICollection<ReportWorkShop>? ReportWorkShops { get; set; }
    }
}
