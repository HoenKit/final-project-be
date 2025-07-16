using final_project_be_Domain.DTOs.Coupon;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface ICouponDAO : IGenericDAO<Coupon>
    {
        Task<List<CouponDto>> GetAllCouponsAsync();
        Task<List<CouponDto>> GetCouponsByCourseIdAsync(int courseId);
        Task<List<CourseCoupon>> GetExpiredByCourseIdAsync(int courseId);
    }

}
