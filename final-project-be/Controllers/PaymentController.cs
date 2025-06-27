using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentRepositoty _paymentRepositoty;
        private readonly ILearningRepository _learnrepository;
        public PaymentController(IPaymentRepositoty paymentRepositoty, ILearningRepository learnrepository)
        {
            _paymentRepositoty = paymentRepositoty;
            _learnrepository = learnrepository;
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

            await _learnrepository.StartCourseAsync(request.UserId, request.CourseId);
            return Ok("Purchase and course start successful.");
        }
    }
}
