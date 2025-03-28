using final_project_be.Data.Models;
using final_project_be.Dtos.Report;
using final_project_be.Interface;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(_ReportCommentRepository.GetReportComment(id));
        }

        // POST api/<ReportCommentController>
        [HttpPost]
        public IActionResult Post([FromBody] ReportCommentDto ReportCommentDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _ReportCommentRepository.CreateReportComment(ReportCommentDto);
            return Ok(ReportCommentDto);
        }

        // PUT api/<ReportCommentController>/5
        [HttpPut]
        public IActionResult Put(ReportCommentDto ReportCommentDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            _ReportCommentRepository.UpdateReportComment(ReportCommentDto);
            return Ok(ReportCommentDto);
        }

        // DELETE api/<ReportCommentController>/5
        [HttpDelete]
        public IActionResult Delete(int reportId, int commentId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _ReportCommentRepository.DeleteReportComment(reportId, commentId);
            return Ok();
        }
    }
}
