using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Report;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportManagerController : Controller
    {
        private readonly IReportRepository _reportRepository;
        public ReportManagerController(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        [HttpGet("by-post/{postId}")]
        public IActionResult GetReportsByPost(int postId)
        {
            var data = _reportRepository.GetReportsByPost(postId);
            return Ok(data);
        }

        [HttpGet("by-user/{userId}")]
        public IActionResult GetReportsByUser(Guid userId)
        {
            var data = _reportRepository.GetReportsByUser(userId);
            return Ok(data);
        }

        [HttpGet("by-comment/{commentId}")]
        public IActionResult GetReportsByComment(int commentId)
        {
            var data = _reportRepository.GetReportsByComment(commentId);
            return Ok(data);
        }

        // GET api/<ReportPostController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _reportRepository.GetReport(id));
        }

        // POST api/<ReportPostController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReportDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _reportRepository.CreateReport(dto);
            return Ok(dto);
        }

        // PUT api/<ReportPostController>/5
        [HttpPut]
        public async Task<IActionResult> Put(ReportDto dto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            await _reportRepository.UpdateReport(dto);
            return Ok(dto);
        }

        // DeleteAsync api/<ReportPostController>/5
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int Id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _reportRepository.DeleteReport(Id);
            return Ok();
        }
    }
}
