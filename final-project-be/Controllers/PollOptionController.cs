using final_project_be.Dtos.PollOption;
using final_project_be.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public IActionResult Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(_PollOptionRepository.GetPollOption(id));
        }

        // POST api/<PollOptionController>
        [HttpPost]
        public IActionResult Post([FromBody] PollOptionDto PollOptionDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _PollOptionRepository.CreatePollOption(PollOptionDto);
            return Ok(PollOptionDto);
        }

        // PUT api/<PollOptionController>/5
        [HttpPut]
        public IActionResult Put(PollOptionDto PollOptionDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            _PollOptionRepository.UpdatePollOption(PollOptionDto);
            return Ok(PollOptionDto);
        }

        // DELETE api/<PollOptionController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _PollOptionRepository.DeletePollOption(id);
            return Ok();
        }
    }
}

