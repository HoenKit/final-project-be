using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Mentor
{
	public class MentorDto
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
    public class CreateMentorDto
    {
        public int MentorId { get; set; }
        public Guid UserId { get; set; }
        public string? Introduction { get; set; }
        public string? JobTitle { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? StudyLevel { get; set; }
        public string? CitizenID { get; set; }
        public string? Signature { get; set; }
        public string? IssuePlace { get; set; }
        public DateTime? ExpiredDate { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public List<MentorCertificateDto>? MentorCertificates { get; set; }
    }
    public class GetMentorDto
    {
        public int MentorId { get; set; }
        public Guid UserId { get; set; }
        public string? Introduction { get; set; }
        public string? JobTitle { get; set; }
        public string? StudyLevel { get; set; }
        public string? CitizenID { get; set; }
        public string? Degree { get; set; }
        public string? Signature { get; set; }
        public string? IssuePlace { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public List<GetMentorCertificateDto>? MentorCertificates { get; set; }
        public int TotalCourses { get; set; } = 0;
        public int TotalStudents { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;
        public decimal AverageRating { get; set; } = 0;
    }

}
