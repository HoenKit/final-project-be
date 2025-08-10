using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.Models;
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

        [HttpGet("by-Course/{courseId}")]
        public IActionResult GetReportsByCourse(int courseId)
        {
            var data = _reportRepository.GetReportsByCourse(courseId);
            return Ok(data);
        }

        [HttpGet("by-WorkShop/{workShopId}")]
        public IActionResult GetReportsByWorkShop(int workShopId)
        {
            var data = _reportRepository.GetReportsByWorkShop(workShopId);
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
    }
}
