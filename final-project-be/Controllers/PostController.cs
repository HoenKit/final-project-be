using final_project_be_Domain.DTOs.Post;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Ultils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

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
        //Update GetAllPost
        [HttpGet]
        public IActionResult GetAll(int? page, int? CategoryId, string? title, Guid? userId)
        {
            int currentPage = page ?? 1;

            var pagedPosts = _postRepository.GetAllPosts(currentPage, 5, CategoryId, title, userId);
            return Ok(pagedPosts);
        }

        // GET api/<PostController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(_postRepository.GetPost(id));
        }


        // POST api/<PostController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostCreateDto postDto)//Change PostDto to PostCreateDto
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var post = await _postRepository.CreatePost(postDto);

            await _hubContext.Clients.All.SendAsync("ReceivePost", post);
            return Ok(postDto);
        }

        // PUT api/<PostController>/5
        [HttpPut]
        public IActionResult Put(PostCreateDto postDto)//Change PostDto to PostCreateDto
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            _postRepository.UpdatePost(postDto);
            return Ok(postDto);
        }

        // DELETE api/<PostController>/5
        [HttpPut("toggle-deleted/{id}")]
        public async Task<IActionResult> TogglePostDeleteStatus(int id)
        {
            var updatedPost = await _postRepository.ToggleIsDeleted(id);
            if (updatedPost == null)
            {
                return StatusCode(500, "Failed to update post status.");
            }
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

            var pagedPosts = _postRepository.GetAllPostsIsDeleted(currentPage, 5, CategoryId, title, userId);
            return Ok(pagedPosts);
        }
    }
}
