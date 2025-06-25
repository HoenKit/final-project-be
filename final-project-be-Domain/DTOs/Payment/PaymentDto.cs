using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Payment
{
    public class PaymentDto
    {
        public Guid UserId { get; set; }
        public int CourseId { get; set; }
        public int CouponId { get; set; }
    }
}
