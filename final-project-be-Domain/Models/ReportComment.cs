using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace final_project_be_Domain.Models
{
    public class ReportComment
    {
        [ForeignKey("Report")]
        public int ReportId { get; set; }
        [ForeignKey("Comment")]
        public int CommentId { get; set; }
        [JsonIgnore]
        public Report? Report { get; set; }
        [JsonIgnore]
        public Comment? Comment { get; set; }
    }
}
