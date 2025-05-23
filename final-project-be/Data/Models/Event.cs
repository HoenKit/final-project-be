using System.ComponentModel.DataAnnotations;

namespace final_project_be.Data.Models
{
    public class Event
    {
        [Key]
        public int EventId { get; set; }
        public string EventName { get; set; }
        public string Decription { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public ICollection<ReportEvent>? ReportEvents { get; set; }
    }
}
