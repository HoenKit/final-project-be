using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace final_project_be_Domain.Models
{
    public class ReportPost
    {
        [ForeignKey("Report")]
        public int ReportId { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }
        [JsonIgnore]
        public Report? Report { get; set; }
        [JsonIgnore]
        public Post? Post { get; set; }
    }
}
