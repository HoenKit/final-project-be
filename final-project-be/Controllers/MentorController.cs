using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Notification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MentorController : ControllerBase
    {
        private readonly IMentorRepository _mentorRepository;
        public MentorController(IMentorRepository mentorRepository)
        {
            _mentorRepository = mentorRepository;
        }
        // GET: api/<NotificationController>
        [HttpGet]
        public  IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedNotifications =  _mentorRepository.GetAllMentors(currentPage, 5);
            return Ok(pagedNotifications);
        }

        // GET api/<NotificationController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _mentorRepository.GetMentorandCertificate(id));
        }

        [HttpGet("get-by-user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _mentorRepository.GetMentorByUserId(userId));
        }

        // POST api/<NotificationController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateMentorDto MentorDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _mentorRepository.CreateMentor(MentorDto);
            return Ok(MentorDto);
        }

        // PUT api/<NotificationController>/5
        [HttpPut]
        public async Task<IActionResult> Put(CreateMentorDto MentorDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            await _mentorRepository.UpdateMentor(MentorDto);
            return Ok(MentorDto);
        }

        // DeleteAsync api/<NotificationController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _mentorRepository.DeleteMentor(id);
            return Ok();
        }
    }
}
