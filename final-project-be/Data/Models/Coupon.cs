using System.ComponentModel.DataAnnotations;

namespace final_project_be.Data.Models
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }
        public float Discount { get; set; }
        public string CouponName { get; set; }
        public ICollection<CourseCoupon>? CourseCoupon { get; set; }
        public PaymentCourse? PaymentCourse { get; set; }
    }
}
