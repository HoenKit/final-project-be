using final_project_be_Application.Ultils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgressController : ControllerBase
    {
        private readonly Caculator _caculator;

        public ProgressController(Caculator caculator)
        {
            _caculator = caculator;
        }
        [HttpGet("course-progress")]
        public IActionResult GetCourseProgress(Guid userId, int courseId)
        {
            var progress = _caculator.CalculateCourseCompletion(userId, courseId);
            return Ok(new { UserId = userId, CourseId = courseId, Percentage = progress });
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
        public IActionResult GetModuleProgress(Guid userId, int moduleId)
        {
            double progress = _caculator.CalculateModuleProgress(userId, moduleId);
            return Ok(new
            {
                UserId = userId,
                ModuleId = moduleId,
                Percentage = progress
            });
        }
    }
}
