using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace final_project_be_Domain.Models
{
    public class ReportUser
    {
        [ForeignKey("Report")]
        public int ReportId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        [JsonIgnore]
        public Report? Report { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
    }
}
