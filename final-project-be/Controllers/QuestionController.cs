using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Module;
using final_project_be_Domain.DTOs.Question;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    [Authorize]
    public class QuestionController : ControllerBase
	{
		private readonly IQuestionRepository _questionRepository;
		public QuestionController(IQuestionRepository questionRepository)
		{
			_questionRepository = questionRepository;
		}
		// GET: api/<QuestionController>
		[HttpGet("get-all-question-by-lesson/{lessonId}")]
		public async Task<IActionResult> GetAllQuestionByLessonId(int lessonId)
		{
			var questions = await _questionRepository.GetAllQuestionByLessonId(lessonId);
			return Ok(questions);
		}

		// GET api/<QuestionController>/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var question = await _questionRepository.GetQuestion(id);
			return Ok(question);
		}

		// POST api/<QuestionController>
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] QuestionDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);
				var question = await _questionRepository.CreateQuestion(dto);
				return Ok(question);
			}
			catch (Exception e)
			{
				return Ok("Error" + e);
			}
		}

		// PUT api/<QuestionController>/5
		[HttpPut]
		public async Task<IActionResult> Put([FromBody] UpdateQuestionDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);
				var question = await _questionRepository.UpdateQuestion(dto);
				return Ok(question);
			}
			catch (Exception e)
			{
				return Ok("Error" + e);
			}
		}

		// DELETE api/<QuestionController>/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			await _questionRepository.DeleteQuestion(id);
			return Ok();
		}

        [HttpPost("upload-excel")]
        public async Task<IActionResult> UploadExcel([FromForm] UploadExcelRequest request)
        {
            if (request.File == null || (request.File.Length == 0))
                return BadRequest("Invalid file");

            await _questionRepository.ImportQuestionsFromExcel(request.File, (request.LessonId));
            return Ok("Imported successfully");
        }

        [HttpPost("import-AI")]
        public async Task<IActionResult> ImportQuizFromAI([FromForm] QuizImportRequest request)
        {
            if (request.PdfFile == null || request.PdfFile.Length == 0)
            {
                return BadRequest("PDF file must not be empty.");
            }

            if (request.LessonId <= 0)
            {
                return BadRequest("LessonId must be positive.");
            }

            if (string.IsNullOrWhiteSpace(request.Difficulty))
            {
                return BadRequest("Difficulty is required (e.g., Easy, Medium, Hard).");
            }

            var result = await _questionRepository.ImportQuizFromAI(
                request.PdfFile,
                request.LessonId,
                request.Number,
                request.Difficulty
            );

            if (result)
                return Ok(new { message = "Quiz imported successfully." });
            else
                return StatusCode(500, new { message = "Failed to import quiz from AI." });
        }

    }
}
