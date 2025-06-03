using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Service.CloudinaryService;
using final_project_be_Domain.DTOs.Courses;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
	public class CourseController : ControllerBase
	{
		private readonly ICourseRepository _courseRepository;
		private readonly CloudinaryService _cloudinaryService;
		public CourseController(ICourseRepository courseRepository, CloudinaryService cloudinaryService)
		{
			_courseRepository = courseRepository;
			_cloudinaryService = cloudinaryService;
		}
		// GET: api/<CourseController>
		[HttpGet]
		public IActionResult GetAll(int? page, int? CategoryId, string? title, Guid? userId)
		{
			int currentPage = page ?? 1;

			var pagedCourses = _courseRepository.GetAllCourses(currentPage, 5, CategoryId, title, userId);
			return Ok(pagedCourses);
		}

		// GET api/<CourseController>/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var course = await _courseRepository.GetCourse(id);
			return Ok(course);
		}

		// POST: CourseController/Create
		[HttpPost]
		public async Task<IActionResult> Post([FromForm] CourseDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);
				var course = await _courseRepository.CreateCourse(dto);
				return Ok(course);
			}
			catch (Exception e)
			{
				return Ok("Error" + e);
			}
		}

		// PUT: CourseController/Edit/5
		[HttpPut]
		public async Task<IActionResult> Put([FromForm] UpdateCourseDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);
				var course = await _courseRepository.UpdateCourse(dto);
				return Ok(course);
			}
			catch (Exception e)
			{
				return Ok("Error" + e);
			}
		}

		// DELETE api/<PostController>/5
		[HttpPut("toggle-deleted/{id}")]
		public async Task<IActionResult> TogglePostDeleteStatus(int id)
		{
			var updatedCourse = await _courseRepository.ToggleIsDeleted(id);
			if (updatedCourse == null)
			{
				return StatusCode(500, "Failed to UpdateAsync course status.");
			}
			return Ok(updatedCourse);
		}

		[HttpPost("upload")]
		public async Task<IActionResult> UploadVideo(IFormFile videoFile)
		{
			try
			{
				var videoUrl = await _cloudinaryService.UploadVideoAndGetUrlAsync(videoFile);

				return Ok(new
				{
					Url = videoUrl
				});
			}
			catch (Exception ex)
			{
				return BadRequest(new { Message = ex.Message });
			}
		}

		[HttpPost("delete-video")]
		public async Task<IActionResult> DeleteVideo(string url)
		{
			var deleted = await _cloudinaryService.DeleteVideoByUrlAsync(url);

			if (deleted)
			{
				return Ok();
			}
			else
			{
				return NotFound();
			}

		}

	}
}
