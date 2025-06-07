using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Service.CloudinaryService;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Post;
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
        public async Task<IActionResult> Post([FromBody] MentorCertificateDto CertificateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _mentorCertificateRepository.CreateMentorCertificate(CertificateDto);
            return Ok(CertificateDto);
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
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _mentorCertificateRepository.DeleteMentorCertificate(id);
            return Ok();
        }
    }
}
