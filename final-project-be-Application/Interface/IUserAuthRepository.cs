using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Users;
using System.Threading.Tasks;


namespace final_project_be_Application.Interface
{
    public interface IUserAuthRepository
    {
        public Task<User> RegisterAsync(UserRegisterDto dto);
        public Task<LoginResultDto> LoginAsync(UserLoginDto dto);
        public Task<UsercurrentDto> GetCurrentUserAsync();
        public Task<User> ConfirmAccountAsync(Guid UserId);
        public Task ForgotPasswordAsync(ForgotpasswordDto dto);
        public Task ResetPasswordAsync(string Token, ResetPasswordDto Request);
        public string ValidateResetToken(string token);
        public Task<LoginResultDto> HandleGoogleLoginAsync(string code);
        public Task LogoutAsync();
    }
}
