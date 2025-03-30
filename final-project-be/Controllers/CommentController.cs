using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.Comment;
using final_project_be.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }
        // GET: api/<CommentController>
        [HttpGet]
        public IActionResult GetAll(int? page, int postId)  
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedComments = _commentRepository.GetAllCommentsByPostId(currentPage, 5, postId);
            return Ok(pagedComments);
        }



        // GET api/<CommentController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(_commentRepository.GetComment(id));
        }

        // POST api/<CommentController>
        [HttpPost]
        public IActionResult Post([FromBody] CommentDto commentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _commentRepository.CreateComment(commentDto);
            return Ok(commentDto);
        }

        // PUT api/<CommentController>/5
        [HttpPut]
        public IActionResult Put(CommentDto commentDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            _commentRepository.UpdateComment(commentDto);
            return Ok(commentDto);
        }

        // DELETE api/<CommentController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _commentRepository.DeleteComment(id);
            return Ok();
        }
    }
}
