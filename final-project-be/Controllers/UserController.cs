using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.DTOs.Users;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public IActionResult GetAll(int? page, int? pageSize)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            int currentSize = pageSize ?? 6;
            int currentPage = page ?? 1;
            var pagedComments = _userRepository.GetAllUsers(currentPage, currentSize);
            return Ok(pagedComments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userRepository.GetUserandUserMetadata(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }

            var userDto = new UserManagerDto
            {
                UserId = user.UserId,
                Email = user.Email,
                Phone = user.Phone,
                Password = user.Password,
                Point = user.Point,
                IsBanned = user.IsBanned,
                CreateAt = user.CreateAt,
                UpdateAt = user.UpdateAt,
				UserMetaData = user.UserMetaData != null
                    ? new UserProfileDto
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        Phone = user.Phone,
                        FirstName = user.UserMetaData.FirstName,
                        LastName = user.UserMetaData.LastName,
                        Birthday = user.UserMetaData.Birthday,
                        Gender = user.UserMetaData.Gender,
                        Address = user.UserMetaData.Address,
						Avatar = user.UserMetaData.Avatar
					}
                    : null
            };

            return Ok(userDto);
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

        [HttpPut]
        public async Task<IActionResult> Put(UserManagerDto usermanagerDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            await _userRepository.UpdateUser(usermanagerDto);
            return Ok(usermanagerDto);
        }

        [HttpPut("update-user-point")]
        public async Task<IActionResult> UpdateUserPoint(decimal point, Guid userId)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            var user = await _userRepository.UpdateUserPoint(point, userId);
            return Ok(user);
        }

        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto dto)
        {
            if (dto == null || dto.UserId == Guid.Empty)
            {
                return BadRequest("Invalid user data.");
            }

            var updatedUser = await _userRepository.UpdateUserProfileAsync(dto);

            if (updatedUser == null)
            {
                return NotFound("User not found.");
            }

            return Ok(updatedUser);
        }

        [HttpGet("monthly-stats")]
        public IActionResult GetUserStatisticsByMonth()
        {
            var stats = _userRepository.GetUserStatisticsByMonth();
            return Ok(stats);
        }
    }
}
