using final_project_be_Domain.DTOs.Report;
using final_project_be_Application.Interface;
using Microsoft.AspNetCore.Mvc;
using final_project_be_Domain.Models;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportCourseController : Controller
    {
        private readonly IReportCourseRepository _ReportCourseRepository;
        public ReportCourseController(IReportCourseRepository ReportCourseRepository)
        {
            _ReportCourseRepository = ReportCourseRepository;
        }
        // GET: api/<ReportCourseController>
        [HttpGet]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedReportCourses = _ReportCourseRepository.GetAllReportCourses(currentPage, 5);
            return Ok(pagedReportCourses);
        }

        // GET api/<ReportCourseController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _ReportCourseRepository.GetReportCourse(id));
        }

        // POST api/<ReportCourseController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReportCourseDto ReportCourseDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _ReportCourseRepository.CreateReportCourse(ReportCourseDto);
            return Ok(ReportCourseDto);
        }

        [HttpGet("grouped")]
        public IActionResult GetGroupedReports(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedReportCourses = _ReportCourseRepository.GetGroupedReportCourses(currentPage, 5);
            return Ok(pagedReportCourses);
        }

        [HttpDelete("DeleteReportsByCourseId/{courseId}")]
        public async Task<IActionResult> DeleteReportsByCourseId(int courseId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _ReportCourseRepository.DeleteReportsByCourseId(courseId);
            return Ok();
        }
    }
}
