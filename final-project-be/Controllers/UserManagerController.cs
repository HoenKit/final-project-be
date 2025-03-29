using final_project_be.Dtos.Comment;
using final_project_be.Dtos.User;
using final_project_be.Interface;
using final_project_be.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagerController : ControllerBase
    {
        private readonly IUserManagerRepository _usermanagerRepository;
        public UserManagerController(IUserManagerRepository usermanagerRepository)
        {
            _usermanagerRepository = usermanagerRepository;
        }

        [HttpPut("toggle-ban/{userId}")]
        public async Task<IActionResult> ToggleUserBanStatus(Guid userId)
        {
            var updatedUser = await _usermanagerRepository.ToggleIsBanned(userId);
            if (updatedUser == null)
            {
                return StatusCode(500, "Failed to update user status.");
            }
            return Ok(updatedUser);
        }

        [HttpGet]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedComments = _usermanagerRepository.GetAllUsers(currentPage, 5);
            return Ok(pagedComments);
        }

        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(_usermanagerRepository.GetUser(id));
        }

        [HttpPut]
        public IActionResult Put(UserManagerDto usermanagerDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            _usermanagerRepository.UpdateUser(usermanagerDto);
            return Ok(usermanagerDto);
        }
        [HttpPut("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto dto)
        {
            if (dto == null || dto.UserId == Guid.Empty)
            {
                return BadRequest("Invalid user data.");
            }

            var updatedUser = await _usermanagerRepository.UpdateUserProfileAsync(dto);

            if (updatedUser == null)
            {
                return NotFound("User not found.");
            }

            return Ok(updatedUser);
        }
    }
}
