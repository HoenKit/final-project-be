using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Lesson;
using final_project_be_Domain.DTOs.Module;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class LessonController : ControllerBase
	{
		private readonly ILessonRepository _lessonRepository;
		public LessonController(ILessonRepository lessonRepository)
		{
			_lessonRepository = lessonRepository;
		}
		// GET: api/<LessonController>
		[HttpGet]
		public async Task<IActionResult> GetAllModulesByCourseId(int moduleId)
		{
			var lessons = await _lessonRepository.GetAllLessonByModuleId(moduleId);
			return Ok(lessons);
		}

		// GET api/<LessonController>/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var lesson = await _lessonRepository.GetLesson(id);
			return Ok(lesson);
		}

		// POST api/<LessonController>
		[HttpPost]
		public async Task<IActionResult> Post([FromForm] LessonDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);
				var lesson = await _lessonRepository.CreateLesson(dto);
				return Ok(lesson);
			}
			catch (Exception e)
			{
				return Ok("Error" + e);
			}
		}

		// PUT api/<LessonController>/5
		[HttpPut]
		public async Task<IActionResult> Put([FromForm] UpdateLessonDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);
				var lesson = await _lessonRepository.UpdateLesson(dto);
				return Ok(lesson);
			}
			catch (Exception e)
			{
				return Ok("Error" + e);
			}
		}

		// DELETE api/<LessonController>/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			await _lessonRepository.DeleteLesson(id);
			return Ok();
		}
	}
}
