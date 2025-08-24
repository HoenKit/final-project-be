using final_project_be_Domain.DTOs.Assignment;
using final_project_be_Domain.DTOs.Category;
using final_project_be_Domain.DTOs.Mentor;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Courses
{
	public class GetCourseDto
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
        public bool IsDeleted { get; set; }
		public string? Status { get; set; }
		public Decimal? AverageRating { get; set; }
		public decimal? TotalReviews { get; set; }
        public DateTime? CreateAt { get; set; }
        public string MeetingLink { get; set; }
        public MentorDto? Mentor { get; set; }
        public AssignmentDto? Assignment { get; set; }
	}

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StatusEnum
    {
        Pending,
        Approved,
        Rejected
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum LevelEnum
    {
        Beginner,
        Intermediate,
        Advanced
    }
}
