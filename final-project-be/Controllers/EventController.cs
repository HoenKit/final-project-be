using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        private readonly IEventRepository _EventRepository;

        public EventController(IEventRepository eventRepository)
        {
            _EventRepository = eventRepository;
        }

        [HttpPost("add-points")]
        public async Task<IActionResult> AddPoints([FromBody] AddPointsDto dto)
        {
            var result = await _EventRepository.AddPointsAsync(dto.UserId, dto.Points);
            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(new
            {
                result.User.UserId,
                result.User.Email ,
                result.User.Point,
                result.User.Turns
            });
        }
        [HttpPost("add-turns")]
        public async Task<IActionResult> AddTurns( Guid userId)
        {
            var result = await _EventRepository.DailyLoginAsync(userId);
            if (!result.Success)
                return NotFound(result.Message);

            return Ok(new
            {
                result.User?.UserId,
                result.User.Email,
                result.User.Turns
            });
        }
    }
}
