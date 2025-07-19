using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Workshop;
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkshop(int id, [FromBody] WorkShopDto dto)
        {
            if (dto == null || dto.WorkShopId != id)
                return BadRequest("Workshop information is invalid.");

            var updated = await _workshopRepository.UpdateWorkshop(dto);

            if (updated == null)
                return NotFound($"No Workshop found with ID: {id}");

            var returnDto = _mapper.Map<WorkShopDto>(updated);
            return Ok(returnDto);
        }
    }
}
