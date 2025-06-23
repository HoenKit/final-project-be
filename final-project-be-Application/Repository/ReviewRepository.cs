using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Review;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        private readonly ReviewDAO _reviewDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<ReviewRepository> _logger;
        public ReviewRepository(ReviewDAO reviewDAO, IMapper mapper, ILogger<ReviewRepository> logger) : base(reviewDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _reviewDAO = reviewDAO;
        }

        public Task<Review> CreateReview(ReviewDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteReview(int id)
        {
            throw new NotImplementedException();
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
                var averageRating = totalCount > 0 ? Math.Round(query.Average(r => r.rate), 1) : 0;
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
                        rate = r.rate,
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

        public Task<Review> UpdateReview(UpdateReviewDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
