using System.ComponentModel.DataAnnotations;

namespace final_project_be.Data.Models
{
    public class MembershipPlan
    {
        [Key]
        public int PlanId { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<PaymentPlan>? PaymentPlans { get; set; }
    }
}
