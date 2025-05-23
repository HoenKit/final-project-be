using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class UserSchedule
    {
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        [ForeignKey("Schedule")]
        public int ScheduleId { get; set; }
        public Schedule? Schedule { get; set; }
        public User? User { get; set; }
    }
}
