using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class ReportPost
    {
        [ForeignKey("Report")]
        public int ReportId { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; } 
        public Report? Report { get; set; }
        public Post? Post { get; set; }
    }
}
