using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } 
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public decimal? Point { get; set; } = 0;
        public bool IsBanned { get; set; } = false;
        public bool IsConfirmed { get; set; } = false;
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        public UserMetadata? UserMetaData { get; set; }
        public Mentor? Mentor { get; set; }
        public Review? Review { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<PollOptionVote>? PollOptionVotes { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<UserRole>? UserRoles { get; set; }
        public ICollection<Report>? Reports { get; set; }
        public ICollection<ReportUser>? ReportUsers { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }
        public ICollection<UserAssignment>? UserAssignments { get; set; }
        public ICollection<UserSchedule>? UserSchedules { get; set; }
        public ICollection<UserModule>? UserModules { get; set; }
        public ICollection<UserCourse>? UserCourses { get; set; }
        public ICollection<UserLesson>? UserLessons { get; set; }
        public ICollection<UserWorkshop>? UserWorkshops { get; set; }
        public ICollection<Payment>? Payments { get; set; }
        public ICollection<Messages>? Messages { get; set; }
    }
}
