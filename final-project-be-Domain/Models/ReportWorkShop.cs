using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace final_project_be_Domain.Models
{
    public class ReportWorkShop
    {
        [ForeignKey("WorkShop")]
        public int WorkshopId { get; set; }
        [ForeignKey("Report")]
        public int ReportId { get; set; }
        [JsonIgnore]
        public WorkShop? WorkShop { get; set; }
        [JsonIgnore]
        public Report? Report { get; set; }
    }
}
