using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Module;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class AnswerController : ControllerBase
	{
		private readonly IAnswerRepository _answerRepository;
		public AnswerController(IAnswerRepository answerRepository)
		{
			_answerRepository = answerRepository;
		}
		// GET: api/<AnswerController>
		[HttpGet("get-all-answer-by-question/{questionId}")]
		public async Task<IActionResult> GetAllAnswerByQuestionId(int questionId)
		{
			var answers = await _answerRepository.GetAllAnswerByQuestionId(questionId);
			return Ok(answers);
		}

		// GET api/<AnswerController>/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var answer = await _answerRepository.GetAnswer(id);
			return Ok(answer);
		}

		// POST api/<AnswerController>
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] AnswerDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);
				var answer = await _answerRepository.CreateAnswer(dto);
				return Ok(answer);
			}
			catch (Exception e)
			{
				return Ok("Error" + e);
			}
		}

		// PUT api/<AnswerController>/5
		[HttpPut]
		public async Task<IActionResult> Put([FromBody] UpdateAnswerDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);
				var answer = await _answerRepository.UpdateAnswer(dto);
				return Ok(answer);
			}
			catch (Exception e)
			{
				return Ok("Error" + e);
			}
		}

		// DELETE api/<AnswerController>/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			await _answerRepository.DeleteAnswer(id);
			return Ok();
		}
	}
}
