using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.DTOs.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("toggle-ban/{userId}")]
        public async Task<IActionResult> ToggleUserBanStatus(Guid userId)
        {
            var updatedUser = await _userRepository.ToggleIsBanned(userId);
            if (updatedUser == null)
            {
                return StatusCode(500, "Failed to UpdateAsync user status.");
            }
            return Ok(updatedUser);
        }



        [Authorize]
        [HttpPut("update-avatar")]
        public async Task<IActionResult> UpdateAvatar(Guid userId, [FromForm] UpdateAvatarDto dto)
        {
            try
            {
                if (dto.Avatar == null || dto.Avatar.Length == 0)
                    return BadRequest("Avatar file is required.");

                var avatarUrl = await _userRepository.UpdateAvatarAsync(userId, dto);

                if (avatarUrl == null)
                    return NotFound("User not found");

                return Ok(new
                {
                    Message = "Avatar updated successfully",
                    AvatarUrl = avatarUrl
                });
            }
            catch (ArgumentException ex)
            {
                
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("userCertificate")]
        public async Task<IActionResult> GetCertificatesByUser(Guid userId)
        {
            if (userId == Guid.Empty)
                return BadRequest("UserId is required.");

            var certificates = await _userRepository.GetCertificatesByUserIdAsync(userId);

            if (!certificates.Any())
                return NotFound("No completed certificates found for this user.");

            return Ok(certificates);
        }

        [HttpGet]
        public IActionResult GetAll(int? page, int? pageSize)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int currentSize = pageSize ?? 100;
            int currentPage = page ?? 1;
            var pagedComments = _userRepository.GetAllUsers(currentPage, currentSize);
            return Ok(pagedComments);
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserId(Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userRepository.GetUserandUserMetadata(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(user);
        }
        [Authorize]
        [HttpPut("update/{userId}")]
        public async Task<IActionResult> Update(Guid userId, [FromBody] UpdateUserMetadataDto dto)
        {
            bool success = await _userRepository.UpdateMetadataAsync(userId, dto);
            if (!success)
                return NotFound(new { message = "User metadata not found" });

            return Ok(new { message = "User metadata updated successfully" });
        }

        [Authorize]
        [HttpPut("update-user-point")]
        public async Task<IActionResult> UpdateUserPoint(decimal point, Guid userId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var user = await _userRepository.UpdateUserPoint(point, userId);
            return Ok(user);
        }

        [Authorize]
        [HttpGet("monthly-stats")]
        public IActionResult GetUserStatisticsByMonth()
        {
            var stats = _userRepository.GetUserStatisticsByMonth();
            return Ok(stats);
        }
    }
}
