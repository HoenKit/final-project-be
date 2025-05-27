using System.ComponentModel.DataAnnotations;

namespace final_project_be_Domain.DTOs.User
{
	public class UserDto
	{
		public Guid UserId { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Password { get; set; }
	}
}
