using final_project_be_Domain.Models;

namespace final_project_be_Domain.DTOs.Users
{
	public class UserRegisterDto
	{
		public string Email { get; set; }
		public string Password { get; set; }
		public string ConfirmPassword { get; set; }
		public string Phone { get; set; }
		public UserMetadataDto userMetadataDto { get; set; }
	}

    public class UserLoginResultDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
    public class UserMetadataDto
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateTime? Birthday { get; set; }
		public string Gender { get; set; }
		public string Address { get; set; }
	}

    public class LoginResultDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? ErrorMessage { get; set; }
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
    public class GoogleTokenRequest
    {
        public string IdToken { get; set; } = string.Empty;
    }
    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public DateTime ExpiresAt { get; set; }
    }
    public class GoogleUserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Picture { get; set; } = string.Empty;
        public string Sub { get; set; } = string.Empty;
        public bool EmailVerified { get; set; }
    }
    public class ForgotpasswordDto
	{
        public string Email { get; set; }
    }

	public class ResetPasswordDto
    {
        public string NewPassword { get; set; }
	}
}
