using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Coupon;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponRepository _couponRepository;

        public CouponController(ICouponRepository couponRepository)
        {
            _couponRepository = couponRepository;
        }

        // GET: api/coupons
        [HttpGet]
        public async Task<IActionResult> GetAllCoupons()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var coupons = await _couponRepository.GetAllCouponsAsync();
            return Ok(coupons);
        }

        // GET: api/coupons/by-course/5
        [HttpGet("by-course/{courseId}")]
        public async Task<IActionResult> GetCouponsByCourseId(int courseId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var coupons = await _couponRepository.GetCouponsByCourseIdAsync(courseId);
            return Ok(coupons);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourseCoupon([FromBody] CreateCouponDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid input");

            var courseCoupon = new CourseCoupon
            {
                CourseId = dto.CourseId,
                CouponId = dto.CouponId,
                ExpiredAt = dto.ExpiredAt
            };

            var result = await _couponRepository.CreateCourseCouponAsync(courseCoupon);

            if (!result)
                return StatusCode(500, "Failed to create course coupon.");

            return Ok(new
            {
                message = "CourseCoupon created successfully.",
                dto.CourseId,
                dto.CouponId,
                dto.ExpiredAt
            });
        }

    }
}
