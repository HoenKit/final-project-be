using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Notification;
using final_project_be_Domain.DTOs.Payment;
using final_project_be_Domain.DTOs.Transaction;
using final_project_be_Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepositoty _paymentRepositoty;
        private readonly ILearningRepository _learnrepository;
        private readonly INotificationRepository _notificationRepository;
        public PaymentController(IPaymentRepositoty paymentRepositoty, ILearningRepository learnrepository, INotificationRepository notificationRepository)
        {
            _paymentRepositoty = paymentRepositoty;
            _learnrepository = learnrepository;
            _notificationRepository = notificationRepository;
        }
        [HttpPost("buy-course")]
        public async Task<IActionResult> BuyCourse([FromBody] PaymentDto request)
        {
            var result = await _paymentRepositoty.BuyCourseAsync(request.UserId, request.CourseId, request.CouponId);

            if (!result.Success)
            {
                return result.Error switch
                {
                    "NotEnoughPoint" => BadRequest("Not enough points to purchase the course."),
                    "NotFound" => NotFound("User or course not found."),
                    "MentorNotFound" => NotFound("Mentor not found."),
                    "PreviouslyPurchased" => Conflict("You have already purchased this course."),
                    "InactiveCourse" => BadRequest("Course is not available for purchase."),
                    _ => StatusCode(500, "Unexpected error occurred while purchasing the course.")
                };
            }


            await _notificationRepository.CreateNotification(new NotificationDto
            {
                UserId = request.UserId, 
                Message = $"You have successfully purchased the course. "
            });

            await _learnrepository.StartCourseAsync(request.UserId, request.CourseId);
            return Ok("Purchase and course start successful.");
        }


        [HttpPost("buy-premium")]
        public async Task<IActionResult> BuyPremium([FromBody] BuyPremiumRequest request)
        {
            try
            {
                await _paymentRepositoty.BuyPremiumAsync(request.UserId, request.PlanId);
                return Ok(new { Message = "Membership purchased successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        [HttpGet("Premium-package")]
        public async Task<IActionResult> GetAllMembershipPlans()
        {
            var plans = await _paymentRepositoty.GetAllMembershipplanAsync();

            if (plans == null || !plans.Any())
            {
                return NotFound(new { message = "No membership plans found" });
            }

            return Ok(plans);
        }

        [HttpGet]
        public IActionResult GetAll(int? page, int? pageSize, Guid? userId, string? sortOption, [FromQuery] List<ServiceTypeEnum>? ServiceType)
        {
            int currentPage = page ?? 1;
            int currentSize = pageSize ?? 50;

            var pagedTransactions = _paymentRepositoty.GetAll(currentPage, currentSize, userId, sortOption, ServiceType);
            return Ok(pagedTransactions);
        }

        [HttpGet("monthly-stats")]
        public async Task<IActionResult> GetStatisticsByMonth([FromQuery] int? year = null)
        {
            try
            {
                var stats = await _paymentRepositoty.GetStatisticsByMonth(year);
                return Ok(stats);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log error here
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
