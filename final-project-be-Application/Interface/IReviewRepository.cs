using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Review;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface IReviewRepository : IRepository<Review>
    {
        public Task<Review> CreateReview(ReviewDto dto);
        public Task<bool> DeleteReview(int id);
        public Task<Review> GetReview(int id);
        public Task<Review> UpdateReview(UpdateReviewDto dto);
        public CourseReviewPageResult GetAllReviewsByCourseId(int courseId, int page, int pageSize);
    }
}
