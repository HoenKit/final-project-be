using final_project_be_Domain.DTOs.Post;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Ultils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using final_project_be_Domain.Models;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : Controller
    {
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly IPostRepository _postRepository;
        public PostController(IPostRepository postRepository, IHubContext<SignalRHub> hubContext)
        {
            _postRepository = postRepository;
            _hubContext = hubContext;
        }
        // GET: api/<PostController>
        //UpdateAsync GetAllPost
        [HttpGet]
        public IActionResult GetAll(int? page, int? CategoryId, string? title, Guid? userId)
        {
            int currentPage = page ?? 1;

            var pagedPosts = _postRepository.GetAllPosts(currentPage, 5, CategoryId, title, userId, false);
            return Ok(pagedPosts);
        }

        // GET api/<PostController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _postRepository.GetPost(id));
        }


        // POST api/<PostController>
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] PostCreateDto postDto)//Change PostDto to PostCreateDto
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var post = await _postRepository.CreatePost(postDto);

            await _hubContext.Clients.All.SendAsync("ReceivePost", post);
            return Ok(post);
        }

        // PUT api/<PostController>/5
        [HttpPut]
        public async Task<IActionResult> Put([FromForm] PostCreateDto postDto)//Change PostDto to PostCreateDto
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var post = await _postRepository.UpdatePost(postDto);
            await _hubContext.Clients.All.SendAsync("ReceivePost", post);
            return Ok(post);
        }

        // DELETE api/<PostController>/5
        [HttpPut("toggle-deleted/{id}")]
        public async Task<IActionResult> TogglePostDeleteStatus(int id)
        {
            var updatedPost = await _postRepository.ToggleIsDeleted(id);
            if (updatedPost == null)
            {
                return StatusCode(500, "Failed to UpdateAsync post status.");
            }
            await _hubContext.Clients.All.SendAsync("ReceivePost", updatedPost);
            return Ok(updatedPost);
        }


        [HttpGet("monthly-stats")]
        public IActionResult GetPostStatisticsByMonth()
        {
            var stats = _postRepository.GetPostStatisticsByMonth();
            return Ok(stats);
        }

        [HttpGet("GetAllIsDeleted")]
        public IActionResult GetAllIsDeleted(int? page, int? CategoryId, string? title, Guid? userId)
        {
            int currentPage = page ?? 1;

            var pagedPosts = _postRepository.GetAllPosts(currentPage, 5, CategoryId, title, userId, true);
            return Ok(pagedPosts);
        }

        [HttpGet("GetDetail/{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _postRepository.GetPostDetail(id));
        }
    }
}
