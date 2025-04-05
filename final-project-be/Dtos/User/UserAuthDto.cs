using final_project_be.Data.Models;

namespace final_project_be.Dtos.User
{
    public class UserRegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Phone { get; set; }
        public UserMetadataDto userMetadataDto { get; set; }
    }
    public class UserMetadataDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
    }
    public class UserLoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UsercurrentDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
    }
}
