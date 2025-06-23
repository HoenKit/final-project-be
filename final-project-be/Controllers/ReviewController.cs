using final_project_be_Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewController(IReviewRepository reviewRepository)
        {
            _reviewRepository = reviewRepository;
        }

        [HttpGet("get-by-course/{courseId}")]
        public IActionResult GetAllReviewsByCourseId(int courseId, int? page, int? pageSize)
        {
            int currentPage = page ?? 1;
            int currentSize = pageSize ?? 6;
            var result = _reviewRepository.GetAllReviewsByCourseId(courseId, currentPage, currentSize);

            return Ok(new
            {
                averageRating = result.AverageRating,
                reviewCount = result.ReviewCount,
                totalCount = result.TotalCount,
                page = result.Page,
                pageSize = result.PageSize,
                reviews = result.Reviews
            });
        }
    }
}
