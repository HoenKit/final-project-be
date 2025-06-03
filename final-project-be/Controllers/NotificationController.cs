using final_project_be_Domain.DTOs.Notification;
using final_project_be_Application.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationRepository _NotificationRepository;
        public NotificationController(INotificationRepository NotificationRepository)
        {
            _NotificationRepository = NotificationRepository;
        }
        // GET: api/<NotificationController>
        [HttpGet]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedNotifications = _NotificationRepository.GetAllNotifications(currentPage, 5);
            return Ok(pagedNotifications);
        }



        // GET api/<NotificationController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _NotificationRepository.GetNotification(id));
        }

        // POST api/<NotificationController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NotificationDto NotificationDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _NotificationRepository.CreateNotification(NotificationDto);
            return Ok(NotificationDto);
        }

        // PUT api/<NotificationController>/5
        [HttpPut]
        public async Task<IActionResult> Put(NotificationDto NotificationDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            await _NotificationRepository.UpdateNotification(NotificationDto);
            return Ok(NotificationDto);
        }

        // DeleteAsync api/<NotificationController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _NotificationRepository.DeleteNotification(id);
            return Ok();
        }
    }
}
