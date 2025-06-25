using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Review;
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

        [HttpGet("get-by-review/{courseId}")]
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

        // POST: ReviewController/Create
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] ReviewDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var review = await _reviewRepository.CreateReview(dto);
                return Ok(review);
            }
            catch (Exception e)
            {
                return Ok("Error" + e);
            }
        }

        // PUT: ReviewController/Edit/5
        [HttpPut]
        public async Task<IActionResult> Put([FromForm] UpdateReviewDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var review = await _reviewRepository.UpdateReview(dto);
                return Ok(review);
            }
            catch (Exception e)
            {
                return Ok("Error" + e);
            }
        }

        // DELETE api/<ReviewController>/5
        [HttpPut("toggle-deleted/{id}")]
        public async Task<IActionResult> ToggleReviewDeleteStatus(int id)
        {
            var updatedReview = await _reviewRepository.DeleteReview(id);
            if (updatedReview == null)
            {
                return StatusCode(500, "Failed to UpdateAsync review status.");
            }
            return Ok(updatedReview);
        }
    }
}
