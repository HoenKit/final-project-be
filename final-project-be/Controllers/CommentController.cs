using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Comment;
using final_project_be_Application.Interface;
using final_project_be_Application.Ultils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using final_project_be_Application.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IHubContext<SignalRHub> _hubContext;
        public CommentController(ICommentRepository commentRepository, IHubContext<SignalRHub> hubContext)
        {
            _commentRepository = commentRepository;
            _hubContext = hubContext;
        }
        // GET: api/<CommentController>

        [HttpGet("GetByPostId")]
        public IActionResult GetAllByPostId(int? page, int postId)  
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedComments = _commentRepository.GetAllCommentsByPostId(currentPage, 5, postId);
            return Ok(pagedComments);
        }

        // GET api/<CommentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _commentRepository.GetComment(id));
        }

        // POST api/<CommentController>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CommentDto commentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var comment = await _commentRepository.CreateComment(commentDto);
            await _hubContext.Clients.All.SendAsync("ReceiveComment", comment);
            return Ok(commentDto);
        }

        // PUT api/<CommentController>/5
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> Put(CommentDto commentDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            await _commentRepository.UpdateComment(commentDto);
            return Ok(commentDto);
        }

        // DeleteAsync api/<CommentController>/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _commentRepository.DeleteComment(id);
            return Ok();
        }

        [HttpGet("GetAllComments")]
        public IActionResult GetAll(int? page, int? pageSize)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int currentSize = pageSize ?? 100;
            int currentPage = page ?? 1;
            var pagedComments = _commentRepository.GetAllComments(currentPage, currentSize);
            return Ok(pagedComments);
        }
    }
}
