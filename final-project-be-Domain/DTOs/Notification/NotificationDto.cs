using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.DTOs.Notification
{
	public class NotificationDto
	{
		public int NotificationId { get; set; }
		public Guid UserId { get; set; }
		public string Message { get; set; }
	}
}
