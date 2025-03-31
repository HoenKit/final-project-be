using final_project_be.Interface;
using final_project_be.Repository;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportManagerController : Controller
    {
        private readonly IReportPostRepository _reportPostRepository;
        private readonly IReportCommentRepository _reportCommentRepository;
        private readonly IReportUserRepository _reportUserRepository;
        public ReportManagerController(IReportPostRepository reportPostRepository, IReportCommentRepository reportCommentRepository, IReportUserRepository reportUserRepository)
        {
            _reportPostRepository = reportPostRepository;
            _reportCommentRepository = reportCommentRepository;
            _reportUserRepository = reportUserRepository;
        }

        [HttpGet("ReportComment")]
        public IActionResult GetAllReportComment(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedReportComments = _reportCommentRepository.GetAllReportComments(currentPage, 5);
            return Ok(pagedReportComments);
        }

        [HttpGet("ReportPost")]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedReportPosts = _reportPostRepository.GetAllReportPosts(currentPage, 5);
            return Ok(pagedReportPosts);
        }

        [HttpGet("ReportUser")]
        public IActionResult GetAllReportUser(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedReportPosts = _reportUserRepository.GetAllReportUsers(currentPage, 5);
            return Ok(pagedReportPosts);
        }
    }
}
