using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using final_project_be_Application.Interface;
using final_project_be_Application.Service.CloudinaryService;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Review;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Application.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private readonly IReviewDAO _reviewDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewRepository> _logger;
        public ReviewRepository(IReviewDAO reviewDAO, IMapper mapper, ILogger<ReviewRepository> logger) : base(reviewDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _reviewDAO = reviewDAO;
        }

        public async Task<Review> CreateReview(ReviewDto dto)
        {
            try
            {
                await _reviewDAO.BeginTransactionAsync();
                var review = _mapper.Map<Review>(dto);
                await _reviewDAO.AddAsync(review);
                await _reviewDAO.CommitTransactionAsync();
                _logger.LogInformation("Add Review success");
                return review;
            }
            catch (Exception ex)
            {
                await _reviewDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding Review");
                return null;
            }
        }

        public async Task<Review> DeleteReview(int id)
        {
            try
            {
                await _reviewDAO.BeginTransactionAsync();

                var review = await _reviewDAO.GetByIdAsync(id);
                if (review == null)
                {
                    _logger.LogWarning("Review not found with ID: {Id}", id);
                    await _reviewDAO.RollbackTransactionAsync();
                    return null;
                }

                review.IsDeleted = !review.IsDeleted;
                review.UpdateAt = DateTime.Now;

                await _reviewDAO.UpdateAsync(review);
                await _reviewDAO.CommitTransactionAsync();

                _logger.LogInformation("Toggle IsDeleted success for review ID: {Id}", id);
                return review;
            }
            catch (Exception ex)
            {
                await _reviewDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when toggling IsDeleted for review ID: {Id}", id);
                return null;
            }
        }

        public CourseReviewPageResult GetAllReviewsByCourseId(int courseId, int page, int pageSize)
        {
            try
            {
                var query = _reviewDAO.GetAll()
                    .Include(x => x.User)
                        .ThenInclude(r => r.UserMetaData)
                    .Where(p => !p.IsDeleted && p.CourseId == courseId);

                var totalCount = query.Count();
                var averageRating = totalCount > 0 ? Math.Round(query.Average(r => r.Rate), 1) : 0;
                var reviewCount = totalCount;

                var reviews = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(r => new ReviewResponseDto
                    {
                        ReviewId = r.ReviewId,
                        CourseId = r.CourseId,
                        UserId = r.UserId,
                        Content = r.Content,
                        IsDeleted = r.IsDeleted,
                        Rate = r.Rate,
                        CreateAt = r.CreateAt,
                        UpdateAt = r.UpdateAt,
                        User = r.User
                    })
                    .ToList();

                _logger.LogInformation("Get Reviews success");

                return new CourseReviewPageResult(reviews, totalCount, page, pageSize, averageRating, reviewCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting Reviews");
                return new CourseReviewPageResult(new List<ReviewResponseDto>(), 0, page, pageSize, 0, 0);
            }
        }

        public Task<Review> GetReview(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<Review> UpdateReview(UpdateReviewDto dto)
        {
            try
            {
                await _reviewDAO.BeginTransactionAsync();
                var review = await _reviewDAO.GetByIdAsync(dto.ReviewId);
                if (review == null)
                {
                    _logger.LogWarning("Review not found with ID: {Id}", dto.ReviewId);
                    await _reviewDAO.RollbackTransactionAsync();
                    return null;
                }
                _mapper.Map(dto, review);
                await _reviewDAO.UpdateAsync(review);
                await _reviewDAO.CommitTransactionAsync();
                _logger.LogInformation("Update Review success");
                return review;
            }
            catch (Exception ex)
            {
                await _reviewDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when updating Review");
                return null;
            }
        }
    }
    
}
