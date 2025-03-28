using final_project_be.Dtos.Category;
using final_project_be.Dtos.PollOption;
using final_project_be.Interface;
using final_project_be.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollOptionController : ControllerBase
    {
        private readonly IPollOptionRepository _pollOptionRepository;
        public PollOptionController(IPollOptionRepository pollOptionRepository)
        {
            _pollOptionRepository = pollOptionRepository;
        }
        // GET: api/<PollOptionController>
        [HttpGet]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedComments = _pollOptionRepository.GetAllPollOptions(currentPage, 5);
            return Ok(pagedComments);
        }

        // GET api/<PollOptionController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(_pollOptionRepository.GetPollOption(id));
        }

        // POST api/<PollOptionController>
        [HttpPost]
        public IActionResult Post([FromBody] PollOptionDto pollOptionDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _pollOptionRepository.CreatePollOption(pollOptionDto);
            return Ok(pollOptionDto);
        }

        // PUT api/<PollOptionController>/5
        [HttpPut]
        public IActionResult Put(PollOptionDto pollOptionDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            _pollOptionRepository.UpdatePollOption(pollOptionDto);
            return Ok(pollOptionDto);
        }

        // DELETE api/<PollOptionController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _pollOptionRepository.DeletePollOption(id);
            return Ok();
        }
    }
}
