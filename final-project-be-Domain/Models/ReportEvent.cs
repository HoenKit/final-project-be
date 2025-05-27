using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace final_project_be_Domain.Models
{
    public class ReportEvent
    {
        [ForeignKey("Event")]
        public int EventId { get; set; }
        [ForeignKey("Report")]
        public int ReportId { get; set; }
        [JsonIgnore]
        public Event? Event { get; set; }
        [JsonIgnore]
        public Report? Report { get; set; }
    }
}
