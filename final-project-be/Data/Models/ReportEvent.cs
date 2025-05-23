using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class ReportEvent
    {
        [ForeignKey("Event")]
        public int EventId { get; set; }
        [ForeignKey("Report")]
        public int ReportId { get; set; }
        public Event? Event { get; set; }
        public Report? Report { get; set; }
    }
}
