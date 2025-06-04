using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Application.Interface;
using Microsoft.AspNetCore.Mvc;
using final_project_be_Application.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportCommentController : ControllerBase
    {
        private readonly IReportCommentRepository _ReportCommentRepository;
        public ReportCommentController(IReportCommentRepository ReportCommentRepository)
        {
            _ReportCommentRepository = ReportCommentRepository;
        }
        // GET: api/<ReportCommentController>
        [HttpGet]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedReportComments = _ReportCommentRepository.GetAllReportComments(currentPage, 5);
            return Ok(pagedReportComments);
        }



        // GET api/<ReportCommentController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _ReportCommentRepository.GetReportComment(id));
        }

        // POST api/<ReportCommentController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReportCommentDto ReportCommentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _ReportCommentRepository.CreateReportComment(ReportCommentDto);
            return Ok(ReportCommentDto);
        }

        // PUT api/<ReportCommentController>/5
        [HttpPut]
        public async Task<IActionResult> Put(ReportCommentDto ReportCommentDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            await _ReportCommentRepository.UpdateReportComment(ReportCommentDto);
            return Ok(ReportCommentDto);
        }

        // DeleteAsync api/<ReportCommentController>/5
        [HttpDelete]
        public async Task<IActionResult> DeleteAsync(int reportId, int commentId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _ReportCommentRepository.DeleteReportComment(reportId, commentId);
            return Ok();
        }

        [HttpGet("grouped")]
        public IActionResult GetGroupedReports(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedReportPosts = _ReportCommentRepository.GetGroupedReportComments(currentPage, 5);
            return Ok(pagedReportPosts);
        }
    }
}
