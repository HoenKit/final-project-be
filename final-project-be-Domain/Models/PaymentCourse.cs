using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
    public class PaymentCourse
    {
        

        [ForeignKey("payment")]
        public int PaymentId { get; set; }
        [ForeignKey("Courses")]
        public int CourseId { get; set; }
        [ForeignKey("Coupon")]
        public int CouponId { get; set;}
        public DateTime CreatedAt { get; set; }
        public Coupon? Coupon { get; set; }
        public Payment? Payment { get; set; }
        public Courses? Courses { get; set; }
    }
}
