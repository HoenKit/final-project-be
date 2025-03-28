using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } 
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int Point { get; set; }
        public bool IsBanned { get; set; } = false;
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        public UserMetadata? UserMetaData { get; set; }
        public ICollection<Post>? Posts { get; set; }
        public ICollection<PollOptionVote>? PollOptionVotes { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<UserRole>? UserRoles { get; set; }
        public ICollection<ReportUser>? ReportUsers { get; set; }
    }
}
