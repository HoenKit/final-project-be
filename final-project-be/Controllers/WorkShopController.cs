using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Category;
using final_project_be_Domain.DTOs.Workshop;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkShopController : ControllerBase
    {
        private readonly IWorkshopRepository _workshopRepository;
        private readonly IMapper _mapper;

        public WorkShopController(IWorkshopRepository workshopRepository, IMapper mapper)
        {
            _workshopRepository = workshopRepository;
            _mapper = mapper;
        }
        [Authorize(Roles ="Mentor")]
        [HttpPost]
        public async Task<IActionResult> CreateWorkshop([FromBody] WorkShopCreateDto dto)
        {
            if (dto == null)
                return BadRequest("Invalid workshop data");

            var result = await _workshopRepository.CreateWorkshopAsync(dto);

            if (result == null)
                return NotFound("MentorId does not exist or error when creating workshop");

            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAllWorkshop([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = _workshopRepository.GetAllWorkshop(page, pageSize);
            return Ok(result);
        }

        // GET api/<LessonController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var lesson = await _workshopRepository.GetWorkshop(id);
            return Ok(lesson);
        }


        // PUT api/<CategoryController>/5
        [Authorize(Roles ="Mentor")]
        [HttpPut]
        public async Task<IActionResult> Put(WorkShopDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            await _workshopRepository.UpdateWorkshop(dto);
            return Ok(dto);
        }
        // DeleteAsync api/<CategoryController>/5
        [Authorize(Roles = "Mentor")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _workshopRepository.DeleteWorkshop(id);
            return Ok();
        }
    }
}
