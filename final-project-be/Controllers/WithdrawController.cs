using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Withdraw;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WithdrawController : ControllerBase
    {
        private readonly IWithdrawRepository _withdrawRepository;
        private readonly ILogger<WithdrawController> _logger;

        public WithdrawController(IWithdrawRepository withdrawRepository, ILogger<WithdrawController> logger)
        {
            _withdrawRepository = withdrawRepository;
            _logger = logger;
        }
        [Authorize(Roles = "Mentor")]
        [HttpPost]
        public async Task<IActionResult> CreateWithdraw([FromBody] WithdrawDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _withdrawRepository.CreateWithdraw(dto);

            if (result == null)
                return StatusCode(500, "An error occurred while creating the withdraw.");

            return Ok(result);
        }
        [Authorize(Roles = "Admin,Mentor")]
        [HttpGet]
        public IActionResult GetAllWithdraw(int page, int pageSize, int? mentorId, string? sortOption, [FromQuery] List<WithdrawEnum>? status)
        {
            var result = _withdrawRepository.GetAllWithdraw(page, pageSize, mentorId, sortOption, status);
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("status/{withdrawId}")]
        public async Task<IActionResult> UpdateStatus(int withdrawId, string status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return BadRequest("Status is required.");

            var result = await _withdrawRepository.UpdateStatus(withdrawId, status);

            if (result == null)
                return NotFound($"Withdraw with ID {withdrawId} not found or update failed.");

            return Ok(result);
        }
    }
}
