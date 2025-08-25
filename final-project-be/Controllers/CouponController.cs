using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Coupon;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using Microsoft.AspNetCore.Authorization;
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

        /// <summary>
        /// Add coupons to multiple courses
        /// </summary>
        [Authorize(Roles ="Mentor")]
        [HttpPost("add-coupons")]
        public async Task<IActionResult> AddCourseCoupons([FromBody] AddCouponDto dto)
        {
            if (dto == null || dto.CourseIds == null || !dto.CourseIds.Any())
            {
                return BadRequest(new { message = "The CourseIds list cannot be left blank." });
            }

            try
            {
                await _couponRepository.AddCourseCouponsAsync(dto);
                return Ok(new { message = "Successfully added coupon to course." });
            }
            catch (Exception ex)
            {
                // log exception nếu cần
                return StatusCode(500, new { message = "An error occurred.", detail = ex.Message });
            }
        }

    }
}
