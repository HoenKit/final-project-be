using final_project_be_Domain.DTOs.PollOption;
using final_project_be_Application.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PollOptionController : ControllerBase
    {
        private readonly IPollOptionRepository _PollOptionRepository;
        public PollOptionController(IPollOptionRepository PollOptionRepository)
        {
            _PollOptionRepository = PollOptionRepository;
        }
        // GET: api/<PollOptionController>
        [HttpGet]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedPollOptions = _PollOptionRepository.GetAllPollOptions(currentPage, 5);
            return Ok(pagedPollOptions);
        }



        // GET api/<PollOptionController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _PollOptionRepository.GetPollOption(id));
        }

        // POST api/<PollOptionController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PollOptionDto PollOptionDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _PollOptionRepository.CreatePollOption(PollOptionDto);
            return Ok(PollOptionDto);
        }

        // PUT api/<PollOptionController>/5
        [HttpPut]
        public async Task<IActionResult> Put(PollOptionDto PollOptionDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            await _PollOptionRepository.UpdatePollOption(PollOptionDto);
            return Ok(PollOptionDto);
        }

        // DeleteAsync api/<PollOptionController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _PollOptionRepository.DeletePollOption(id);
            return Ok();
        }
    }
}

