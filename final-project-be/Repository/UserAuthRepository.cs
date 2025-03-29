using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos.User;
using final_project_be.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace final_project_be.Repository
{
    public class UserAuthRepository : IUserAuthRepository
    {
        private readonly UserManagerDAO _userManagerDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<UserAuthRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserAuthRepository(UserManagerDAO userManagerDAO, IMapper mapper, ILogger<UserAuthRepository> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _userManagerDAO = userManagerDAO;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> LoginAsync(UserLoginDto dto)
        {
            try
            {
                var user = _userManagerDAO.GetUserbyEmail(dto);
                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found with email: {Email}", dto.Email);
                    return null;
                }

                var passwordVerificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, dto.Password);
                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    _logger.LogWarning("Login failed: Invalid password for email: {Email}", dto.Email);
                    return null;
                }

                var token = GenerateToken(user);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = false, 
                    Secure = true,  
                    SameSite = SameSiteMode.Strict, 
                    Expires = DateTime.UtcNow.AddHours(1) 
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", token, cookieOptions);

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    if (_httpContextAccessor.HttpContext.Request.Cookies.ContainsKey("AccessToken"))
                    {
                        _httpContextAccessor.HttpContext.Response.Cookies.Delete("AccessToken");
                    }


                    _logger.LogInformation("User has been logged out successfully.");
                }
                else
                {
                    _logger.LogWarning("HttpContext is null. Unable to clear cookies.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during logout.");
            }
        }

        public async Task<User> RegisterAsync(UserRegisterDto dto)
        {
            _userManagerDAO.BeginTransaction();
            try
            {
                var userExists = _userManagerDAO.UserRegisterExist(dto);
                if (userExists)
                {
                    _logger.LogWarning("Registration failed: User with email {Email} already exists.", dto.Email);
                    return null;
                }

                var user = _mapper.Map<User>(dto);
                var hashedPassword = new PasswordHasher<User>().HashPassword(user, dto.Password);
                user.Email = dto.Email;
                user.Password = hashedPassword;

                _userManagerDAO.Add(user);
                _userManagerDAO.SaveChanges();

                var defaultRole = _userManagerDAO.GetRoleByName("User");
                if (defaultRole != null)
                {
                    var userRole = new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = defaultRole.RoleId
                    };

                    _userManagerDAO.AddUserRole(userRole);
                    _userManagerDAO.SaveChanges(); 
                }

                _userManagerDAO.CommitTransaction();
                _logger.LogInformation("User registered successfully with email: {Email}", dto.Email);
                return user;
            }
            catch (Exception ex)
            {
                _userManagerDAO.RollbackTransaction();
                _logger.LogError(ex, "Failed to register user with email: {Email}", dto.Email);
                return null;
            }
        }


        private string GenerateToken(User user)
        {
            // Define JWT claims
            var roles = _userManagerDAO.GetRolesByUserId(user.UserId);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create JWT token
            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }



    }
}
