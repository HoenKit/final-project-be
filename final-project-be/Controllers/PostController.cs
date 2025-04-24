using final_project_be.Dtos.Post;
using final_project_be.Interface;
using final_project_be.Ultils;
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
        [HttpGet]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedPosts = _postRepository.GetAllPosts(currentPage, 5);
            return Ok(pagedPosts);
        }

        // GET api/<PostController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(_postRepository.GetPost(id));
        }

        //Update SearchPosts
        // GET: api/Post/search
        [HttpGet("search")]
        public IActionResult SearchPosts([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Search query cannot be empty.");
            }

            var posts = _postRepository.SearchPosts(query);
            return Ok(posts);
        }
        //Update get post by userId
        //GET: api/Post/user/userId
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(Guid userId, int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            int pageSize = 5;

            var pagedPosts = await _postRepository.GetPostsByUserId(userId, currentPage, pageSize);

            if (pagedPosts == null || !pagedPosts.Items.Any())
            {
                return NotFound($"No posts found by user {userId}.");
            }

            return Ok(pagedPosts);
        }

        // POST api/<PostController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PostDto postDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var post = await _postRepository.CreatePost(postDto);

            await _hubContext.Clients.All.SendAsync("ReceivePost", post);
            return Ok(postDto);
        }

        // PUT api/<PostController>/5
        [HttpPut]
        public IActionResult Put(PostDto postDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            _postRepository.UpdatePost(postDto);
            return Ok(postDto);
        }

        // DELETE api/<PostController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _postRepository.DeletePost(id);
            return Ok();
        }
    }
}
