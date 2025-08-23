using final_project_be_Domain.DTOs.Coupon;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface ICouponRepository : IRepository<Coupon>
    {
        public Task<List<CouponDto>> GetAllCouponsAsync();
        public Task<List<CouponDto>> GetCouponsByCourseIdAsync(int courseId);
        public Task AddCourseCouponsAsync(AddCouponDto dto);
    }
}
