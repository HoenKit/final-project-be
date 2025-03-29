using final_project_be.Dtos.Report;
using final_project_be.Interface;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(_ReportPostRepository.GetReportPost(id));
        }

        // POST api/<ReportPostController>
        [HttpPost]
        public IActionResult Post([FromBody] ReportPostDto ReportPostDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _ReportPostRepository.CreateReportPost(ReportPostDto);
            return Ok(ReportPostDto);
        }

        // PUT api/<ReportPostController>/5
        [HttpPut]
        public IActionResult Put(ReportPostDto ReportPostDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            _ReportPostRepository.UpdateReportPost(ReportPostDto);
            return Ok(ReportPostDto);
        }

        // DELETE api/<ReportPostController>/5
        [HttpDelete]
        public IActionResult Delete(int reportId, int PostId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _ReportPostRepository.DeleteReportPost(reportId, PostId);
            return Ok();
        }
    }
}
