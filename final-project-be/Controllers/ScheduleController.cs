using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Schedule;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleRepository _Schedulerepository;
        private readonly ILogger<ScheduleController> _logger;
        public ScheduleController(IScheduleRepository Schedulerepository, ILogger<ScheduleController> logger)
        {
            _Schedulerepository = Schedulerepository;
            _logger = logger;
        }


        /// <summary>
        /// Mentor creates schedule
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateSchedule([FromBody] ScheduleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _Schedulerepository.CreateScheduleAsync(dto);

            if (!result)
            {
                _logger.LogWarning("Failed to create schedule");
                return StatusCode(500, "Failed to create schedule");
            }

            return Ok(new { message = "Schedule created successfully" });
        }

        /// <summary>
        /// User selects course schedule (if already taking that course)
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserToSchedule([FromBody] UserScheduleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _Schedulerepository.RegisterUserToScheduleAsync(dto);

            if (!result)
            {
                _logger.LogWarning("User failed to register to schedule or not eligible");
                return BadRequest("User cannot register to this schedule");
            }

            return Ok(new { message = "User registered successfully" });
        }

        [HttpGet("by-course/{courseId}")]
        public async Task<IActionResult> GetSchedulesByCourse(int courseId)
        {
            var schedules = await _Schedulerepository.GetSchedulesByCourseAsync(courseId);
            if (schedules == null || schedules.Count == 0)
                return NotFound($"No schedules found for courseId {courseId}");

            return Ok(schedules);
        }
    }
}
