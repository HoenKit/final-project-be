using final_project_be_Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.DTOs.Users
{
	public class UserManagerDto
	{
		public Guid UserId { get; set; }
		public string Email { get; set; }
		public string Phone { get; set; }
		public string Password { get; set; }
		public decimal? Point { get; set; }
		public bool IsBanned { get; set; }
        public bool IsPremium { get; set; }
        public DateTime CreateAt { get; set; }
		public DateTime? UpdateAt { get; set; } = DateTime.Now;
		public UserProfileDto UserMetaData { get; set; }
	}
}
