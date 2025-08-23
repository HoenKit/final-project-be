using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportWorkShopController : Controller
    {
        private readonly IReportWorkShopRepository _ReportWorkShopRepository;
        public ReportWorkShopController(IReportWorkShopRepository ReportWorkShopRepository)
        {
            _ReportWorkShopRepository = ReportWorkShopRepository;
        }
        // GET: api/<ReportWorkShopController>
        [HttpGet]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedReportWorkShops = _ReportWorkShopRepository.GetAllReportWorkShops(currentPage, 5);
            return Ok(pagedReportWorkShops);
        }

        // GET api/<ReportWorkShopController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _ReportWorkShopRepository.GetReportWorkShop(id));
        }

        // POST api/<ReportWorkShopController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ReportWorkShopDto ReportWorkShopDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _ReportWorkShopRepository.CreateReportWorkShop(ReportWorkShopDto);
            return Ok(ReportWorkShopDto);
        }

        [HttpGet("grouped")]
        public IActionResult GetGroupedReports(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedReportWorkShops = _ReportWorkShopRepository.GetGroupedReportWorkShops(currentPage, 5);
            return Ok(pagedReportWorkShops);
        }

        [HttpDelete("DeleteReportsByWorkShopId/{workShopId}")]
        public async Task<IActionResult> DeleteReportsByWorkShopId(int workShopId)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _ReportWorkShopRepository.DeleteReportsByWorkShopId(workShopId);
            return Ok();
        }
    }
}
