using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Service.CloudinaryService;
using final_project_be_Domain.DTOs.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
	[ApiController]
    public class CourseController : ControllerBase
	{
		private readonly ICourseRepository _courseRepository;
		private readonly ICloudinaryService _cloudinaryService;
		private readonly IModuleRepository _moduleRepository;
		public CourseController(ICourseRepository courseRepository, ICloudinaryService cloudinaryService, IModuleRepository moduleRepository)
		{
			_courseRepository = courseRepository;
			_cloudinaryService = cloudinaryService;
			_moduleRepository = moduleRepository;
		}
		// GET: api/<CourseController>
		[HttpGet]
		public IActionResult GetAll(int? page, int? pageSize, int? CategoryId, string? title, Guid? userId, string? sortOption, int? mentorId, string? Language, string? Level, decimal? MinCost, decimal? MaxCost, decimal? MinRate, decimal? MaxRate, [FromQuery] List<StatusEnum>? statuses)
		{
			int currentPage = page ?? 1;
			int currentSize = pageSize ?? 100;

			var pagedCourses = _courseRepository.GetAllCourses(currentPage, currentSize, CategoryId, title, userId, sortOption, mentorId, Language, Level, MinCost, MaxCost, MinRate, MaxRate, statuses);
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
		[Authorize(Roles ="Mentor")]
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

		[Authorize]
        [HttpGet("status")]
        public async Task<IActionResult> GetUserCoursesByStatus([FromQuery] Guid userId, [FromQuery] string? status)
        {
            if (userId == Guid.Empty)
                return BadRequest("UserId is required");

            var courses = await _courseRepository.GetUserCoursesByStatusAsync(userId, status);
            return Ok(courses);
        }


        // PUT: CourseController/Edit/5
        [Authorize(Roles ="Mentor")]
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

		[Authorize(Roles = "Admin")]
        [HttpPut("toggle-status")]
        public async Task<IActionResult> ToggleCourseStatus(int id, string status)
        {
            var updatedCourse = await _courseRepository.ToggleStatus(id, status);
            if (updatedCourse == null)
            {
                return StatusCode(500, "Failed to UpdateAsync course status.");
            }
            return Ok(updatedCourse);
        }

        // DELETE api/<PostController>/5
        [Authorize(Roles = "Admin,Mentor")]
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

		

		[Authorize]
        [HttpGet("recommend-course")]
        public async Task<IActionResult> RecommendCourses([FromQuery] Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("UserId is required.");

            var recommendations = await _courseRepository.RecommendCoursesAsync(userId);

            return Ok(recommendations);
        }

		[Authorize(Roles ="Mentor")]
        [HttpPost("generate-structure/{courseId}")]
        public async Task<IActionResult> GenerateCourseStructure(int courseId)
        {
            var success = await _moduleRepository.GenerateAndSaveModulesAsync(courseId);
            if (!success)
            {
                return BadRequest("Could not generate modules and lessons for this course.");
            }

            return Ok(new { message = "Course structure generated successfully." });
        }
        [Authorize]
        [HttpGet("monthly-stats/{userId}")]
        public async Task<IActionResult> GetStatisticsByMonth(Guid userId, [FromQuery] int? year = null)
        {
            try
            {
                var stats = await _courseRepository.GetStatisticsByMonth(userId, year);
                return Ok(stats);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                // Log error here
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

    }
}
