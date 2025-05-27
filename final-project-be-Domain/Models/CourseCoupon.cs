using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
    public class CourseCoupon
    {
        [ForeignKey("Coupons")]
        public int CouponId { get; set; }
        [ForeignKey("Courses")]
        public int CourseId { get;  set; }
        public DateTime? ExpiredAt { get; set; }
        public Coupon? Coupons { get; set; }
        public Courses? Courses { get; set; }
    }
}
