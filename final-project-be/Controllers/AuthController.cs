using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Users;
using final_project_be_Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var loginResult = await _userAuthRepository.LoginAsync(loginDto);

            if (!loginResult.Success)
                return BadRequest(new { message = loginResult.ErrorMessage });

            return Ok(new { token = loginResult.Token });
        }

        [HttpPut("ConfirmUser")]
        public async Task<IActionResult> ConfirmUser(Guid UserId)
        {
            var updatedUser = await _userAuthRepository.ConfirmAccountAsync(UserId);
            if (updatedUser == null)
            {
                return StatusCode(500, "Failed to UpdateAsync user status.");
            }
            return Ok(updatedUser);
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

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotpasswordDto forgotpasswordDto)
        {
            try
            {
                await _userAuthRepository.ForgotPasswordAsync(forgotpasswordDto);
                return Ok("Reset password email sent.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string Token, [FromBody] ResetPasswordDto request)
        {
            try
            {
                await _userAuthRepository.ResetPasswordAsync(Token, request);
                return Ok("Password reset successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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
