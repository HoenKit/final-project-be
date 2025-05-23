using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class PaymentPlan
    {
        [ForeignKey("Payment")]
        public int PaymentId { get; set; }
        [ForeignKey("MembershipPlan")]
        public int PlanId { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public DateTime? CreatedAt { get; set;}
        public Payment? Payment { get; set; }
        public MembershipPlan? MembershipPlan { get; set; }
    }
}
