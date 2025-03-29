using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Dtos.Report
{
    public class ReportPostDto
    {
        public int ReportId { get; set; }
        public int PostId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
    }
}
