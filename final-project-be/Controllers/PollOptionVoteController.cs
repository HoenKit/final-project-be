using final_project_be.Dtos.PollOption;
using final_project_be.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class PollOptionVoteController : ControllerBase
    {
        private readonly IPollOptionVoteRepository _polloptionvoteRepository;
        public PollOptionVoteController(IPollOptionVoteRepository polloptionvoteRepository)
        {
            _polloptionvoteRepository = polloptionvoteRepository;
        }
        // GET: api/<PollOptionVoteController>
        [HttpGet]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedComments = _polloptionvoteRepository.GetAllPollOptionVotes(currentPage, 5);
            return Ok(pagedComments);
        }

        // GET api/<PollOptionVoteController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(_polloptionvoteRepository.GetPollOptionVote(id));
        }

        // POST api/<PollOptionVoteController>
        [HttpPost]
        public IActionResult Post([FromBody] PollOptionVoteDto pollOptionVoteDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _polloptionvoteRepository.CreatePollOptionVote(pollOptionVoteDto);
            return Ok(pollOptionVoteDto);
        }

        // PUT api/<PollOptionVoteController>/5
        [HttpPut]
        public IActionResult Put(PollOptionVoteDto pollOptionVoteDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            _polloptionvoteRepository.UpdatePollOptionVote(pollOptionVoteDto);
            return Ok(pollOptionVoteDto);
        }

        // DELETE api/<PollOptionVoteController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _polloptionvoteRepository.DeletePollOptionVote(id);
            return Ok();
        }
    }
}
