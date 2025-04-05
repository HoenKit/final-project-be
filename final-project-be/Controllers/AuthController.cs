using final_project_be.Data.Models;
using final_project_be.Dtos.User;
using final_project_be.Interface;
using final_project_be.Repository;
using final_project_be.Ultils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserAuthRepository _userAuthRepository;
        private readonly Validate _validate;
        public AuthController(IUserAuthRepository userAuthRepository, Validate validate)
        {
            _userAuthRepository = userAuthRepository;
            _validate = validate;
        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(UserRegisterDto registerDto) {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var user = await _userAuthRepository.RegisterAsync(registerDto);
            return Ok("Register Success");
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(UserLoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var token = await _userAuthRepository.LoginAsync(loginDto);
            if (token == null)
                return BadRequest(new { message = "Invalid username or password" });

            return Ok(new { token });
        }

        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_validate.IsValidToken() == false)
                return Unauthorized(new { message = "No token found, please provide a valid token" });

            var userDto = await _userAuthRepository.GetCurrentUserAsync();

            if (userDto == null)
                return NotFound(new { message = "User not found" });

            return Ok(userDto);

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
