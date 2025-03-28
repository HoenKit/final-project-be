using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class ReportUser
    {
        [ForeignKey("Report")]
        public int ReportId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; } 
        public Report? Report { get; set; }
        public User? User { get; set; }
    }
}
