using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class PollOptionVote
    {
        [Key]
        public int OptionVoteId { get; set; }
        [ForeignKey("PollOption")]
        public int PollOptionId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public bool? IsDeleted { get; set; } = false;
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        public PollOption? PollOption { get; set; }
        public User? User { get; set; }
    }
}
