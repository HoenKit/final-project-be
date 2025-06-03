using final_project_be_Domain.DTOs.Post;
using final_project_be_Application.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PostFileController : ControllerBase
	{
		private readonly IPostFileRepository _PostFileRepository;
		public PostFileController(IPostFileRepository PostFileRepository)
		{
			_PostFileRepository = PostFileRepository;
		}
		// GET: api/<PostFileController>
		[HttpGet]

		public IActionResult GetAll(int postId)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);

			var postfiles = _PostFileRepository.GetAllPostFilesByPostId(postId);
			return Ok(postfiles);
		}

		// GET api/<PostFileController>/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			return Ok(await _PostFileRepository.GetPostFile(id));
		}

		// POST api/<PostFileController>
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] PostFileDto PostFileDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			await _PostFileRepository.CreatePostFile(PostFileDto);
			return Ok(PostFileDto);
		}

		// PUT api/<PostFileController>/5
		//[HttpPut]
		//public IActionResult Put(PostFileDto PostFileDto)
		//{
		//	if (!ModelState.IsValid) { return BadRequest(ModelState); }
		//	_PostFileRepository.UpdatePostFile(PostFileDto);
		//	return Ok(PostFileDto);
		//}

		// DeleteAsync api/<PostFileController>/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteAsync(int id)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			await _PostFileRepository.DeletePostFile(id);
			return Ok();
		}
	}
}
