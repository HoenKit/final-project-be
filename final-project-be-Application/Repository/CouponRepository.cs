using AutoMapper;
using DocumentFormat.OpenXml.InkML;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Coupon;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace final_project_be_Application.Repository
{
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        private readonly ICouponDAO _CouponDAO;
        private readonly ICourseCouponDAO _courseCouponDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<CouponRepository> _logger;

        public CouponRepository(ICouponDAO couponDAO, IMapper mapper, ILogger<CouponRepository> logger, ICourseCouponDAO courseCouponDAO) : base(couponDAO)
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


        public async Task AddCourseCouponsAsync(AddCouponDto dto)
        {
            foreach (var courseId in dto.CourseIds)
            {
                // Lấy coupon đã tồn tại (nếu có)
                var existing = _courseCouponDAO
                    .GetCourseCoupons(courseId, dto.CouponId)
                    .FirstOrDefault();

                if (existing == null)
                {
                    // Nếu chưa có -> thêm mới
                    var courseCoupon = new CourseCoupon
                    {
                        CourseId = courseId,
                        CouponId = dto.CouponId,
                        ExpiredAt = dto.ExpiredAt
                    };

                    await _courseCouponDAO.AddCourseCouponAsync(courseCoupon);
                }
                else
                {
                    // exits -> update ExpiredAt
                    existing.ExpiredAt = dto.ExpiredAt;
                    await _courseCouponDAO.UpdateAsync(existing);
                }
            }

            await _courseCouponDAO.SaveChangesAsync();
        }
    }
}
