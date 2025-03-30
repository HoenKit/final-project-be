using final_project_be.Data.Models;
using final_project_be.Dtos.User;

namespace final_project_be.Interface
{
    public interface IUserAuthRepository
    {
        public Task<User> RegisterAsync(UserRegisterDto dto);
        public Task<string> LoginAsync(UserLoginDto dto);
        public Task LogoutAsync();
    }
}
