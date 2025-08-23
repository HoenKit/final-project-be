using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Module;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ModuleController : ControllerBase
	{
		private readonly IModuleRepository _moduleRepository;
        public ModuleController(IModuleRepository moduleRepository)
        {
            _moduleRepository = moduleRepository;
        }
        // GET: api/<ModuleController>
        [HttpGet("get-all-module-by-course/{courseId}")]
		public async Task<IActionResult> GetAllModulesByCourseId(int courseId)
		{
			var modules = await _moduleRepository.GetAllModulesByCourseId(courseId);
			return Ok(modules);
		}

		// GET api/<ModuleController>/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
			var module = await _moduleRepository.GetModule(id);
			return Ok(module);
		}

		// POST api/<ModuleController>
		[Authorize(Roles ="Mentor")]
		[HttpPost]
		public async Task<IActionResult> Post([FromBody] ModuleDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);
				var module = await _moduleRepository.CreateModule(dto);
				return Ok(module);
			}
			catch (Exception e)
			{
				return Ok("Error" + e);
			}
		}

        // PUT api/<ModuleController>/5
        [Authorize(Roles = "Mentor")]
        [HttpPut]
		public async Task<IActionResult> Put([FromBody] UpdateModuleDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);
				var module = await _moduleRepository.UpdateModule(dto);
				return Ok(module);
			}
			catch (Exception e)
			{
				return Ok("Error" + e);
			}
		}

		// DELETE api/<ModuleController>/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			await _moduleRepository.DeleteModule(id);
			return Ok();
		}
	}
}
