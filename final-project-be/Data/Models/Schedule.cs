using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }
        [ForeignKey("Mentor")]
        public int MentorId { get; set; }
        public string? ScheduleName { get; set; }
        public DateTime MentorDay { get; set; }
        public DateTime CreateAt { get; set; }
        public ICollection<UserSchedule>? UserSchedules { get; set; }
        public Mentor? Mentor { get; set; }

    }
}
