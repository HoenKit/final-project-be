using final_project_be.Data.Models;

namespace final_project_be.Dtos.User
{
    public class UserManagerDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public int Point { get; set; }
        public bool IsBanned { get; set; } = false;
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
        public UserMetadata? UserMetaData { get; set; }
    }
}
