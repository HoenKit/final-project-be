using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Dtos.Report
{
    public class ReportDto
    {
        public int ReportId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
    }
}
