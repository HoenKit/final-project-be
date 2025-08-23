using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.DTOs.Users
{
	public class UserProfileDto
	{
		public Guid UserId { get; set; }
		public string? Email { get; set; }
		public string? Phone { get; set; }
		public string? FirstName { get; set; }
		public string? LastName { get; set; }
		public DateTime? Birthday { get; set; }
		public string? Gender { get; set; }
		public string? Address { get; set; }
		public string? Avatar { get; set; }
        public string? Nationality { get; set; }
        public string? Level { get; set; }
        public string? Goals { get; set; }
        public string? FavouriteSubject { get; set; }
    }
    public class UserCertificateDto
    {
        public Guid UserId { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string MentorName { get; set; }
        public string? Level { get; set; }
        public string? CertificateLink { get; set; }
        public DateTime CompletedAt { get; set; }
    }
}
