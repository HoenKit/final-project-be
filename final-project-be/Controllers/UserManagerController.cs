﻿using final_project_be.Dtos.Comment;
using final_project_be.Dtos.User;
using final_project_be.Interface;
using final_project_be.Repository;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> Get(Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _usermanagerRepository.GetUserandUserMetadata(id);

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
                UserProfile = user.UserMetaData != null
                    ? new UserProfileDto
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        Phone = user.Phone,
                        FirstName = user.UserMetaData.FirstName,
                        LastName = user.UserMetaData.LastName,
                        Birthday = user.UserMetaData.Birthday,
                        Gender = user.UserMetaData.Gender,
                        Address = user.UserMetaData.Address
                    }
                    : null
            };

            return Ok(userDto);
        }

        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserId(Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _usermanagerRepository.GetUserandUserMetadata(id);

            if (user == null)
            {
                return NotFound($"User with ID {id} not found.");
            }
            return Ok(user);
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
