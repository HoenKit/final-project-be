using final_project_be.Data.Models;
using final_project_be.Dtos.User;
using final_project_be.Interface;
using final_project_be.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserAuthRepository _userAuthRepository;
        public AuthController(IUserAuthRepository userAuthRepository)
        {
            _userAuthRepository = userAuthRepository;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(UserRegisterDto registerDto) {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userAuthRepository.RegisterAsync(registerDto);
            return Ok(user);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(UserLoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var token = await _userAuthRepository.LoginAsync(loginDto);
            if (token == null)
                return BadRequest("Invalid username or password");
            return Ok(token);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _userAuthRepository.LogoutAsync();
                return Ok(new { message = "Logged out successfully" });
 
        }
    }
}
