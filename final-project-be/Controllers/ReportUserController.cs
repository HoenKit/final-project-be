using final_project_be_Domain.DTOs.Report;
using final_project_be_Application.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportUserController : ControllerBase
    {
        private readonly IReportUserRepository _ReportUserRepository;
        public ReportUserController(IReportUserRepository ReportUserRepository)
        {
            _ReportUserRepository = ReportUserRepository;
        }
        // GET: api/<ReportUserController>
        [HttpGet]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedReportuser = _ReportUserRepository.GetAllReportUsers(currentPage, 5);
            return Ok(pagedReportuser);
        }



        // GET api/<ReportUserController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _ReportUserRepository.GetReportUser(id));
        }

        // POST api/<ReportUserController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReportUserDto ReportuserDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _ReportUserRepository.CreateReportUser(ReportuserDto);
            return Ok(ReportuserDto);
        }

        // PUT api/<ReportUserController>/5
        [HttpPut]
        public async Task<IActionResult> Put(ReportUserDto ReportuserDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            await _ReportUserRepository.UpdateReportUser(ReportuserDto);
            return Ok(ReportuserDto);
        }

        // DELETE api/<ReportUserController>/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int reportId, Guid userid)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _ReportUserRepository.DeleteReportUser(reportId, userid);
            return Ok();
        }

        [HttpGet("grouped")]
        public IActionResult GetGroupedReports(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedReportUsers = _ReportUserRepository.GetGroupedReportUsers(currentPage, 5);
            return Ok(pagedReportUsers);
        }
    }
}
