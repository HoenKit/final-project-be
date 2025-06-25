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
            var success = await _paymentRepositoty.BuyCourseAsync(request.UserId, request.CourseId,request.CouponId);

            if (!success)
                return BadRequest("Unable to purchase course. Possibly due to insufficient points or invalid data.");

            await _learnrepository.StartCourseAsync(request.UserId, request.CourseId);
            return Ok("Purchase and start the course successfully.");
        }
    }
}
