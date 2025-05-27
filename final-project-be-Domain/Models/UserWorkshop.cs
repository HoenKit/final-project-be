using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
    public class UserWorkshop
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        [ForeignKey("WorkShop")]
        public int WorkShopId { get; set; }
        public WorkShop? WorkShop { get; set; }
        public User? User { get; set; }

    }
}
