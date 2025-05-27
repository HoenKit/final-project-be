using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
    public class Mentor
    {
        [Key]
        public int MentorId { get; set; }
        [ForeignKey("User")]
        public Guid UserId {  get; set; }
        public string? StudyLevel { get; set; }
        public string? CitizenID { get; set; }
        public int? MentorCertificateId { get; set; }
        public string? Degree { get; set; }
        public string? FontUrl { get; set; }
        public string? BackUrl { get; set; }
        public string? Signature { get; set; }
        public string? IssuePlace { get; set; }
        public DateTime ExpiredDate { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public ICollection<WorkShop>? WorkShops { get; set; }
        public ICollection<MentorCertificate>? MentorCertificates { get; set; }
        public ICollection<Schedule>? Schedules { get; set; }
        public ICollection<Messages>? Messages { get; set; }
        public ICollection<Courses>? Courses { get; set; }
        public User? User { get; set; }
    }
}
