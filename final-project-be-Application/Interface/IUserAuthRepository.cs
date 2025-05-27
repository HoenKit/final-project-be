using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.User;

namespace final_project_be_Application.Interface
{
    public interface IUserAuthRepository
    {
        public Task<User> RegisterAsync(UserRegisterDto dto);
        public Task<string> LoginAsync(UserLoginDto dto);
        public Task<UsercurrentDto> GetCurrentUserAsync();
        public Task LogoutAsync();
    }
}
