using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface ICourseCouponDAO : IGenericDAO<CourseCoupon>
    {
        IQueryable<CourseCoupon> GetCourseCoupons(int courseId, int couponId);
        Task AddCourseCouponAsync(CourseCoupon courseCoupon);
        Task AddCourseCouponsAsync(IEnumerable<CourseCoupon> courseCoupons);
        Task RemoveCourseCouponsAsync(IEnumerable<CourseCoupon> courseCoupons);
    }

}
