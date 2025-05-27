using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
    public class ReportWorkShop
    {
        [ForeignKey("WorkShop")]
        public int WorkshopId { get; set; }
        [ForeignKey("Report")]
        public int ReportId { get; set; }
        public WorkShop? WorkShop { get; set; }
        public Report? Report { get; set; }
    }
}
