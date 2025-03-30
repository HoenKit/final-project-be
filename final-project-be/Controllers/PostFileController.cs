using final_project_be.Dtos.Post;
using final_project_be.Interface;
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
		public IActionResult Get(int id)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			return Ok(_PostFileRepository.GetPostFile(id));
		}

		// POST api/<PostFileController>
		[HttpPost]
		public IActionResult Post([FromBody] PostFileDto PostFileDto)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			_PostFileRepository.CreatePostFile(PostFileDto);
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

		// DELETE api/<PostFileController>/5
		[HttpDelete("{id}")]
		public IActionResult Delete(int id)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			_PostFileRepository.DeletePostFile(id);
			return Ok();
		}
	}
}
