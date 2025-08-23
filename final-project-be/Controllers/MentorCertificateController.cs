using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Service.CloudinaryService;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Post;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class MentorCertificateController : ControllerBase
    {
        private readonly IMentorCertificateRepository _mentorCertificateRepository;
        public MentorCertificateController(IMentorCertificateRepository mentorCertificateRepository)
        {
            _mentorCertificateRepository = mentorCertificateRepository;
        }

        [HttpGet("GetByUserId")]
        public async Task<IActionResult> GetByUserId(Guid userId)
        {
                var certificates = await _mentorCertificateRepository.GetMentorCertificatesByUserId(userId);

                if (certificates == null)
                {
                    return Ok("Error fetching mentor certificates");
                }

                if (!certificates.Any())
                {
                    return NotFound($"No mentor certificates found for userId {userId}");
                }

                // Optionally, select fields to return instead of full entity
                var result = certificates.Select(c => new
                {
                    c.MentorCertificateId,
                    c.MentorId,
                    c.CertificateName,
                    c.FileUrl
                });

                return Ok(result);
        }
            
        // GET: api/<PostFileController>
        [HttpGet]
        public async Task<IActionResult> GetAll(int mentorId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var mentorCertificates = await _mentorCertificateRepository.GetAllMentorCertificatesByMentorId(mentorId);
            return Ok(mentorCertificates);
        }

        // GET api/<PostFileController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _mentorCertificateRepository.GetMentorCertificate(id));
        }

        // POST api/<PostFileController>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromForm] MentorCertificateDto CertificateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var certificate = await _mentorCertificateRepository.CreateMentorCertificate(CertificateDto);

            if (certificate == null)
                return StatusCode(500, "Could not create mentor certificate");

            // Return the saved object with the uploaded file URL
            return Ok(new
            {
                certificate.MentorCertificateId,
                certificate.MentorId,
                certificate.CertificateName,
                certificate.FileUrl
            });
        }

        // PUT api/<PostFileController>/5
        //[HttpPut]
        //public IActionResult Put(PostFileDto PostFileDto)
        //{
        //	if (!ModelState.IsValid) { return BadRequest(ModelState); }
        //	_PostFileRepository.UpdatePostFile(PostFileDto);
        //	return Ok(PostFileDto);
        //}

        // DeleteAsync api/<PostFileController>/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _mentorCertificateRepository.DeleteMentorCertificate(id);
            return Ok();
        }
    }
}
