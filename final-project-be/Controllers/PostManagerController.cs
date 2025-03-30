using final_project_be.Interface;
using final_project_be.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostManagerController : ControllerBase
    {
        private readonly IPostManagerRepository _postManagerRepository;

        public PostManagerController(IPostManagerRepository postManagerRepository)
        {
            _postManagerRepository = postManagerRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPosts([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool prioritizeReports = false)
        {
            var result = await _postManagerRepository.GetAllPosts(page, pageSize, prioritizeReports);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPostsByUser(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool prioritizeReports = false)
        {
            var result = await _postManagerRepository.GetPostsByUser(userId, page, pageSize, prioritizeReports);
            return Ok(result);
        }

        [HttpGet("subcategory/{subCategoryId}")]
        public async Task<IActionResult> GetPostsBySubCategory(int subCategoryId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] bool prioritizeReports = false)
        {
            var result = await _postManagerRepository.GetPostsBySubCategory(subCategoryId, page, pageSize, prioritizeReports);
            return Ok(result);
        }

        [HttpPut("toggle-Delete/{postId}")]
        public async Task<IActionResult> TogglePostDeletedStatus(int postId)
        {
            var updatedPost = await _postManagerRepository.ToggleIsDeleted(postId);
            if (updatedPost == null)
            {
                return StatusCode(500, "Failed to update post status.");
            }
            return Ok(updatedPost);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(_postManagerRepository.GetPost(id));
        }
    }
}
