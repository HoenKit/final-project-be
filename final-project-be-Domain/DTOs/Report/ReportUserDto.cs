namespace final_project_be_Domain.DTOs.Report
{
	public class ReportUserDto
	{
		public Guid UserreportedId { get; set; }
		public int ReportId { get; set; }
		public Guid UserId { get; set; }
		public string Content { get; set; }
	}
}
