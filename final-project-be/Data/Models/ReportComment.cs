using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class ReportComment
    {
        [ForeignKey("Report")]
        public int ReportId { get; set; }
        [ForeignKey("Comment")]
        public int CommentId { get; set; } 
        public Report? Report { get; set; }
        public Comment? Comment { get; set; }
    }
}
