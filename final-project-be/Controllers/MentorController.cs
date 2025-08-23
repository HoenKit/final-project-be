using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Notification;
using final_project_be_Domain.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
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
        // GET: api/<MentorController>
        [HttpGet]
        public  IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedNotifications =  _mentorRepository.GetAllMentors(currentPage, 100);
            return Ok(pagedNotifications);
        }

        [HttpGet("by-course/{courseId}")]
        public async Task<IActionResult> GetMentorByCourse(int courseId)
        {
            var mentor = await _mentorRepository.GetMentorByCourseIdAsync(courseId);
            if (mentor == null)
                return NotFound($"No mentor found for course ID {courseId}");

            return Ok(mentor);
        }

        // GET api/<MentorController>/5
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

        // POST api/<MentorController>
        [Authorize(Roles ="User")]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] CreateMentorDto MentorDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _mentorRepository.CreateMentor(MentorDto);
            return Ok(MentorDto);
        }

        // PUT api/<MentorController>/5
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put(CreateMentorDto MentorDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            await _mentorRepository.UpdateMentor(MentorDto);
            return Ok(MentorDto);
        }
        [Authorize]
        [HttpPut("update/{userId}")]
        public async Task<IActionResult> Update(Guid userId, [FromBody] InfoBank dto)
        {
            bool success = await _mentorRepository.UpdateInfoBankAsync(userId, dto);
            if (!success)
                return NotFound(new { message = "Mentor not found" });

            return Ok(new { message = "Mentor updated successfully" });
        }

        // DeleteAsync api/<MentorController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _mentorRepository.DeleteMentor(id);
            return Ok();
        }
    }
}
