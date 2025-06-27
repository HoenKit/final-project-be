using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Coupon;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        private readonly CouponDAO _CouponDAO;
        private readonly CourseCouponDAO _courseCouponDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<CouponRepository> _logger;

        public CouponRepository(CouponDAO couponDAO, IMapper mapper, ILogger<CouponRepository> logger, CourseCouponDAO courseCouponDAO) : base(couponDAO)
        {
            _CouponDAO = couponDAO;
            _mapper = mapper;
            _logger = logger;
            _courseCouponDAO = courseCouponDAO;
        }

        public async Task<List<CouponDto>> GetAllCouponsAsync()
        {
            return await _CouponDAO.GetAllCouponsAsync();
        }

        public async Task<List<CouponDto>> GetCouponsByCourseIdAsync(int courseId)
        {
            return await _CouponDAO.GetCouponsByCourseIdAsync(courseId);
        }


        public async Task<bool> CreateCourseCouponAsync(CourseCoupon courseCoupon)
        {
            try
            {
                var oldCoupons = _courseCouponDAO.GetCourseCoupons(courseCoupon.CourseId, courseCoupon.CouponId).ToList();
                await _courseCouponDAO.RemoveCourseCouponsAsync(oldCoupons);

                await _courseCouponDAO.AddCourseCouponAsync(courseCoupon);

                await _courseCouponDAO.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}
