using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string ServiceType { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<PaymentCourse>? PaymentCourses { get; set; }
        public User? User { get; set; }
        public ICollection<PaymentPlan>? PaymentPlans { get; set; }
    }
}
