using final_project_be_Domain.DTOs.Coupon;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class CouponDAO : GenericDAO<Coupon>, ICouponDAO
    {
        private readonly ApplicationDbContext _context;

        public CouponDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<CouponDto>> GetAllCouponsAsync()
        {
            return await _context.Coupon
                .Select(c => new CouponDto
                {
                    CouponId = c.CouponId,
                    CouponName = c.CouponName,
                    Discount = c.Discount
                })
                .ToListAsync();
        }

        public async Task<List<CouponDto>> GetCouponsByCourseIdAsync(int courseId)
        {
            return await _context.courseCoupons
                .Where(cc => cc.CourseId == courseId && cc.ExpiredAt > DateTime.UtcNow)
                .Select(cc => new CouponDto
                {
                    CouponId = cc.Coupons.CouponId,
                    CouponName = cc.Coupons.CouponName,
                    Discount = cc.Coupons.Discount
                })
                .ToListAsync();
        }

        public async Task<List<CourseCoupon>> GetExpiredByCourseIdAsync(int courseId)
        {
            return await _context.courseCoupons
                .Where(cc => cc.CourseId == courseId && cc.ExpiredAt < DateTime.UtcNow)
                .ToListAsync();
        }
    }
}
