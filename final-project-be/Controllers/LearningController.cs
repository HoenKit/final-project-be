using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.LearnDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LearningController : ControllerBase
    {
        private readonly ILearningRepository _learnrepository;
        public LearningController(ILearningRepository learnrepository)
        {
            _learnrepository = learnrepository;
        }
        [HttpPost("start")]
        public async Task<IActionResult> StartCourse([FromBody] StartCourseDto dto)
        {
            await _learnrepository.StartCourseAsync(dto.UserId, dto.CourseId);
            return Ok(new { message = "Course started" });
        }

        [HttpPost("complete-lesson")]
        public async Task<IActionResult> CompleteLesson([FromBody] UserLessonDto dto)
        {
            var userLesson = await _learnrepository.CompleteLessonAsync(dto.UserId, dto.LessonId, dto.Mark);

            var result = new UserLessonDto
            {
                UserId = userLesson.UserId,
                LessonId = userLesson.LessonId,
                CompletedAt = DateTime.UtcNow,
                Mark = userLesson.Mark,
                IsPassed = userLesson.IsPassed
            };

            return Ok(result);
        }

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitQuiz([FromBody] SubmitQuizDto dto)
        {
            if (dto == null || dto.AnswerIds == null || !dto.AnswerIds.Any())
                return BadRequest("Invalid input: missing answer selections.");

            try
            {
                var score = await _learnrepository.SubmitQuizAsync(dto);
                bool isPassed = score >= 80;

                return Ok(new
                {
                    score,
                    isPassed
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, details = ex.Message });
            }
        }

        [HttpGet("{lessonId}")]
        public async Task<IActionResult> GetQuizByLessonId(int lessonId)
        {
            try
            {
                var quiz = await _learnrepository.GetQuizByLessonIdAsync(lessonId);
                if (quiz == null || !quiz.Any())
                    return NotFound("Quiz not found or no questions for this lesson.");

                return Ok(quiz);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPost("upload")]
        public async Task<IActionResult> UploadCertificate([FromForm] CertificateUploadDto dto)
        {
            var success = await _learnrepository.UploadCertificateAndSaveLinkAsync(dto);
            if (!success)
                return BadRequest("Course is not completed or upload failed.");

            return Ok("Certificate uploaded and link saved.");
        }
    }
}
