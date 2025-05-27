namespace final_project_be_Domain.DTOs.PollOption
{
	public class PollOptionVoteDto
	{
		public int OptionVoteId { get; set; }
		public int PollOptionId { get; set; }
		public Guid UserId { get; set; }
		public string Message { get; set; }
	}
}
