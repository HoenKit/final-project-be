using Azure.Core;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Assignment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Cmp;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
	[Authorize]
	[Route("api/[controller]")]
	[ApiController]
	public class AssignmentController : ControllerBase
	{
		private readonly IAssignmentRepository _assignmentRepository;
		public AssignmentController(IAssignmentRepository assignmentRepository)
		{
			_assignmentRepository = assignmentRepository;
		}
		// GET: api/<AssignmentController>
		[HttpGet("get-all-assignment-by-lesson/{lessonId}")]
		public async Task<IActionResult> GetAllAssignmentByLessonId(int lessonId)
		{
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var assignments = await _assignmentRepository.GetAllAssignmentByLessonId(lessonId);
			return Ok(assignments);
		}

		// GET api/<AssignmentController>/5
		[HttpGet("{id}")]
		public async Task<IActionResult> Get(int id)
		{
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var assignment = await _assignmentRepository.GetAssignment(id);
			return Ok(assignment);
		}

        // POST api/<AssignmentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AssignmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (assignment, message) = await _assignmentRepository.CreateAssignment(dto);

            if (assignment == null)
                return BadRequest(new { message });

            return CreatedAtAction(nameof(Post), new { id = assignment.AssignmentId }, assignment);
        }

        [HttpGet("by-creator")]
        public async Task<IActionResult> GetAssignmentsByUserId(Guid userId)
        {
            var assignments = await _assignmentRepository.GetAssignmentsBycreatorAsync(userId);

            if (assignments == null || !assignments.Any())
                return NotFound("No assignments found for this user.");

            return Ok(assignments);
        }

        // PUT api/<AssignmentController>/5
        [HttpPut]
		public async Task<IActionResult> Put([FromBody] UpdateAssignmentDto dto)
		{
			try
			{
				if (!ModelState.IsValid)
					return BadRequest(ModelState);
				var assignment = await _assignmentRepository.UpdateAssignment(dto);
				return Ok(assignment);
			}
			catch (Exception e)
			{
				return Ok("Error" + e);
			}
		}

		// DELETE api/<AssignmentController>/5
		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			if (!ModelState.IsValid) return BadRequest(ModelState);
			await _assignmentRepository.DeleteAssignment(id);
			return Ok();
		}
	}
}
