using final_project_be_Domain.DTOs.Coupon;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class CourseCouponDAO : GenericDAO<CourseCoupon>, ICourseCouponDAO
    {
        private readonly ApplicationDbContext _context;

        public CourseCouponDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<CourseCoupon> GetCourseCoupons(int courseId, int couponId)
            => _context.courseCoupons.Where(cc => cc.CourseId == courseId && cc.CouponId == couponId);

        public async Task AddCourseCouponAsync(CourseCoupon courseCoupon)
            => await _context.courseCoupons.AddAsync(courseCoupon);

        public async Task AddCourseCouponsAsync(IEnumerable<CourseCoupon> courseCoupons)
        {
            await _context.courseCoupons.AddRangeAsync(courseCoupons);
        }

        public async Task RemoveCourseCouponsAsync(IEnumerable<CourseCoupon> courseCoupons)
        {
            _context.courseCoupons.RemoveRange(courseCoupons);
            await Task.CompletedTask;
        }
    }

}
