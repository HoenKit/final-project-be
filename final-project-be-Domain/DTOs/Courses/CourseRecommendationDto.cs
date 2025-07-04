using final_project_be_Domain.DTOs.Mentor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Courses
{
    public class CourseRecommendationDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string? CourseContent { get; set; }
        public decimal Cost { get; set; } = 0;
        public string? SkillLearn { get; set; }
        public string? Requirement { get; set; }
        public string? IntendedLearner { get; set; }
        public string? Language { get; set; }
        public string? Level { get; set; }
        public int? StudentCount { get; set; }
        public string? CoursesImage { get; set; }
        public double? CourseLength { get; set; }
        public string? Status { get; set; }
        public Decimal? AverageRating { get; set; }
        public decimal? TotalReviews { get; set; }
        public DateTime? CreateAt { get; set; }
        public MentorDto? Mentor { get; set; }
        public double Score { get; set; }
    }

}
