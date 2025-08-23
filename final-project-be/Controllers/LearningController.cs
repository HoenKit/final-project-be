using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.LearnDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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

        [HttpGet("UserCourse")]
        public async Task<IActionResult> GetUserCourse(Guid userId, int courseId)
        {
            var userCourse = await _learnrepository.GetUserCourseAsync(userId, courseId);

            if (userCourse == null)
            {
                return NotFound(new { message = "UserCourse not found" });
            }

            return Ok(userCourse);
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

        [HttpPost("DoAssignment")]
        public async Task<IActionResult> CreateUserAssignment([FromBody] CreateUserAssignmentDto dto)
        {
            if (dto.UserId == Guid.Empty || dto.AssignmentId <= 0)
                return BadRequest("Invalid input.");

            try
            {
                var result = await _learnrepository.CreateUserAssignmentAsync(dto);

                if (result == null)
                    return Conflict("Cannot create or update assignment: Conditions not met.");
                var response = new UserAssignmentDto
                {
                    UserId = result.UserId,
                    AssignmentId = result.AssignmentId,
                    IsPresented = result.IsPresented,
                    IsScored = result.IsScored
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log lỗi ra console hoặc file
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "Internal server error occurred.");
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
        [HttpPut("upload")]
        public async Task<IActionResult> UploadCertificate([FromForm] CertificateUploadDto dto)
        {
            var fileUrl = await _learnrepository.UploadCertificateAndSaveLinkAsync(dto);

            if (fileUrl == null)
                return NotFound(new { message = "UserCourse not found or course not completed." });

            return Ok(new { message = "The certificate has been uploaded." });
        }

        [HttpGet("submissions")]
        public async Task<ActionResult> GetUserAssignmentsByAssignmentId(int assignmentId)
        {
            var result = await _learnrepository.GetUserAssignmentsByAssignmentIdAsync(assignmentId);
            if (result == null || result.Count == 0)
                return NotFound("No user submissions found for this assignment.");

            return Ok(result);
        }

        [HttpGet("user")]
        public async Task<ActionResult> GetUserAssignment(int assignmentId, Guid userId)
        {
            var result = await _learnrepository.GetUserAssignmentAsync(userId, assignmentId);
            if (result == null)
                return NotFound("No assignment submission found for this user and assignment.");

            return Ok(result);
        }

        [HttpPut("submitAssignment")]
        public async Task<IActionResult> UpdateUserAssignment([FromBody] submitAssignmentDto dto)
        {
            var success = await _learnrepository.UpdateUserAssignmentAsync(dto);
            if (!success)
                return NotFound("User assignment not found.");

            return Ok("User assignment updated successfully.");
        }

        [Authorize(Roles ="Mentor")]
        [HttpGet("not-presented")]
        public async Task<IActionResult> GetNotPresentedAssignments(int assignmentId)
        {
            var assignments = await _learnrepository.ListAssignmentsNotPresentAsync(assignmentId);

            if (assignments == null || !assignments.Any())
                return NotFound("No user assignments found that are not presented.");

            return Ok(assignments);
        }
        [Authorize(Roles = "Mentor")]
        [HttpPut("mark-present")]
        public async Task<IActionResult> MarkUsersAsPresent([FromBody] MarkPresentDto dto)
        {
            var success = await _learnrepository.MarkUsersAsPresentAsync(dto);
            if (!success)
                return NotFound("No matching user assignments found.");

            return Ok("Users marked as present successfully.");
        }

        [Authorize(Roles = "Mentor")]
        [HttpPut("grade")]
        public async Task<IActionResult> GradeSubmission([FromBody] GradeAssignmentDto dto)
        {
            if (dto.Mark < 0 || dto.Mark > 100)
                return BadRequest("Mark must be between 0 and 100.");

            var success = await _learnrepository.GradeSubmissionAsync(dto);
            if (!success)
                return NotFound("UserLesson or UserAssignment not found.");

            return Ok("Graded successfully.");
        }
    }
}
