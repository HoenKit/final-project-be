using System.ComponentModel.DataAnnotations;

namespace final_project_be_Domain.DTOs.Users
{
	public class UserDto
	{
		public Guid UserId { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Password { get; set; }
	}

	public class UpdateUserMetadataDto
    {
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string? Avatar { get; set; }
        public string? Nationality { get; set; }
        public string? Level { get; set; }
        public string? Goals { get; set; }
        public string? FavouriteSubject { get; set; }

    }
}
