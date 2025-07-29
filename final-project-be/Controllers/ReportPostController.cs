using final_project_be_Domain.DTOs.Report;
using final_project_be_Application.Interface;
using Microsoft.AspNetCore.Mvc;
using final_project_be_Domain.Models;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportPostController : Controller
    {
        private readonly IReportPostRepository _ReportPostRepository;
        public ReportPostController(IReportPostRepository ReportPostRepository)
        {
            _ReportPostRepository = ReportPostRepository;
        }
        // GET: api/<ReportPostController>
        [HttpGet]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedReportPosts = _ReportPostRepository.GetAllReportPosts(currentPage, 5);
            return Ok(pagedReportPosts);
        }

        // GET api/<ReportPostController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _ReportPostRepository.GetReportPost(id));
        }

        // POST api/<ReportPostController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReportPostDto ReportPostDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _ReportPostRepository.CreateReportPost(ReportPostDto);
            return Ok(ReportPostDto);
        }

        [HttpGet("grouped")]
        public IActionResult GetGroupedReports(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedReportPosts = _ReportPostRepository.GetGroupedReportPosts(currentPage, 5);
            return Ok(pagedReportPosts);
        }

        [HttpDelete("DeleteReportsByPostId/{postId}")]
        public async Task<IActionResult> DeleteReportsByPostId(int postId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _ReportPostRepository.DeleteReportsByPostId(postId);
            return Ok();
        }
    }
}
