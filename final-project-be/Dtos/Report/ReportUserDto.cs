namespace final_project_be.Dtos.Report
{
    public class ReportUserDto
    {
        public Guid UserreportedId { get; set; }
        public int ReportId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
    }
}
