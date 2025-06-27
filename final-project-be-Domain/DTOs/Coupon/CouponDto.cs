using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Coupon
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string CouponName { get; set; }
        public float Discount { get; set; }
    }
    public class UserCouponDto
    {
        public int CouponId { get; set; }
        public int CourseId { get; set; }
        public DateTime? ExpiredAt { get; set; }
    }
    public class CreateCouponDto
    {
        public int CouponId { get; set; }          
        public int CourseId { get; set; }   
        public DateTime ExpiredAt { get; set; }
    }
}
