using final_project_be_Application.Interface;
using final_project_be_Application.Ultils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressController : ControllerBase
    {
        private readonly Caculator _caculator;
        private readonly ICourseRepository _courseRepository;

        public ProgressController(Caculator caculator, ICourseRepository courseRepository)
        {
            _caculator = caculator;
            _courseRepository = courseRepository;
        }
        [HttpGet("course-progress")]
        public async Task<IActionResult> GetCourseProgress(Guid userId, int courseId)
        {
            var percentage = await _caculator.CalculateCourseCompletion(userId, courseId);

            return Ok(new
            {
                UserId = userId,
                CourseId = courseId,
                Percentage = percentage
            });
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserCourses(Guid userId)
        {
            var result = await _courseRepository.GetUserCoursesAsync(userId);

            if (!result.Any())
                return NotFound(new { message = "No courses found for this user." });

            return Ok(result);
        }

        [HttpGet("quiz-score")]
        public async Task<IActionResult> GetQuizScore(Guid userId, int lessonId)
        {
            try
            {
                var result = await _caculator.CalculateQuizScore(userId, lessonId);
                return Ok(new
                {
                    UserId = userId,
                    LessonId = lessonId,
                    Score = result.Score,
                    IsPassed = result.IsPassed
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("module-progress")]
        public async Task<IActionResult> GetModuleProgress(Guid userId, int moduleId)
        {
            float progress = await  _caculator.CalculateModuleProgress(userId, moduleId);
            return Ok(new
            {
                UserId = userId,
                ModuleId = moduleId,
                Percentage = progress
            });
        }
    }
}
