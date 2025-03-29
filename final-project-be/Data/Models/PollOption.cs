using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class PollOption
    {
        [Key]
        public int PollOptionId { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }
        public string Title { get; set; }
        public string? PollOptionImage { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;

        public Post? Post { get; set; }
        public ICollection<PollOptionVote>? PollOptionVotes { get; set; }
    }
}
