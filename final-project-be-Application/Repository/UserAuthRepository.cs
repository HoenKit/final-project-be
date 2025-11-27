
﻿using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using final_project_be_Application.Interface;
using final_project_be_Application.Service.EmailService;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Users;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace final_project_be_Application.Repository
{
	public class UserAuthRepository : IUserAuthRepository
    {
        private readonly IUserDAO _UserDAO;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly ILogger<UserAuthRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        private readonly ClientSettings _clientSettings;
        private readonly GoogleSettings _googlesetting;
        public UserAuthRepository(IUserDAO userDAO, IMapper mapper, ILogger<UserAuthRepository> logger, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IEmailService emailService, IOptions<ClientSettings> clientoptions, IOptions<GoogleSettings> googlesetting)
        {
            _UserDAO = userDAO;
            _mapper = mapper;
            _logger = logger;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _clientSettings = clientoptions.Value;
            _googlesetting = googlesetting.Value;
        }

        public async Task<LoginResultDto> LoginAsync(UserLoginDto dto)
        {
            try
            {
                var user = _UserDAO.GetUserbyEmail(dto.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login attempt failed: No user found with email '{Email}'.", dto.Email);
                    return new LoginResultDto
                    {
                        Success = false,
                        ErrorMessage = "No account associated with this email."
                    };
                }

                if (user.IsBanned == true)
                {
                    return new LoginResultDto
                    {
                        Success = false,
                        ErrorMessage = "Your account has been banned. Please contact support for more information."
                    };
                }

                if (user.IsConfirmed == false)
                {
                    return new LoginResultDto
                    {
                        Success = false,
                        ErrorMessage = "Your email has not been confirmed. Please check your inbox for a confirmation link."
                    };
                }

                var passwordVerificationResult = new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, dto.Password);
                if (passwordVerificationResult == PasswordVerificationResult.Failed)
                {
                    return new LoginResultDto
                    {
                        Success = false,
                        ErrorMessage = "Incorrect password. Please try again."
                    };
                }

                var token = GenerateToken(user);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = false,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(1)
                };
                _httpContextAccessor.HttpContext.Response.Cookies.Append("AccessToken", token, cookieOptions);

                return new LoginResultDto
                {
                    Success = true,
                    Token = token
                };


            }
            catch (Exception ex)
            {
                return new LoginResultDto
                {
                    Success = false,
                    ErrorMessage = "An unexpected error occurred. Please try again later."
                };
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

        public async Task<UsercurrentDto> GetCurrentUserAsync()
        {
            var token = _httpContextAccessor.HttpContext.Request.Cookies["AccessToken"];
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("No token found");
            }

            try
            {

                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var userIdString = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
                {
                    throw new UnauthorizedAccessException("Invalid token");
                }

                var user = await _UserDAO.GetByIdAsync(userId);
                if (user == null)
                {
                    throw new KeyNotFoundException("User not found");
                }

                var roles =  _UserDAO.GetRolesByUserId(userId);

                return new UsercurrentDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    Roles = roles.ToList(),
                };
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt");
                throw;
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving current user");
                throw new Exception("An error occurred while retrieving the current user");
            }
        }


        public async Task<User> RegisterAsync(UserRegisterDto dto)
        {
            await _UserDAO.BeginTransactionAsync();
            try
            {
                var userExists = _UserDAO.UserRegisterExist(dto);
                if (userExists)
                {
                    _logger.LogWarning("Registration failed: User with email {Email} already exists.", dto.Email);
                    return null;
                }

                var user = _mapper.Map<User>(dto);
                var hashedPassword = new PasswordHasher<User>().HashPassword(user, dto.Password);
                user.Email = dto.Email;
                user.Password = hashedPassword;

                await _UserDAO.AddAsync(user);
                await _UserDAO.SaveChangesAsync();

                var userMeta = new UserMetadata
                {
                    UserId = user.UserId,
                    FirstName = dto.userMetadataDto.FirstName,
                    LastName = dto.userMetadataDto.LastName,
                    Birthday = dto.userMetadataDto.Birthday,
                    Gender = dto.userMetadataDto.Gender,
                    Address = dto.userMetadataDto.Address
                };

                await _UserDAO.AddUserMetaData(userMeta);
                await _UserDAO.SaveChangesAsync();

                var defaultRole = await _UserDAO.GetRoleByNameAsync("User");
                if (defaultRole != null)
                {
                    var userRole = new UserRole
                    {
                        UserId = user.UserId,
                        RoleId = defaultRole.RoleId
                    };

                    await _UserDAO.AddUserRoleAsync(userRole);
                    await _UserDAO.SaveChangesAsync(); 
                }

                var confirmLink = $"{_clientSettings.BaseUrl}ConfirmEmail?UserId={user.UserId}";
                string body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; background-color: #f4f8fb; color: #333;'>
                    <div style='max-width: 600px; margin: auto; background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'>
                        <h2 style='color: #28a745;'>Confirm Your Email</h2>
                        <p>Hello,</p>
                        <p>Thank you for registering. Please confirm your email address by clicking the button below.</p>
                        <div style='text-align: center; margin: 30px 0;'>
                            <a href='{confirmLink}' style='display: inline-block; background-color: #28a745; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-size: 16px;'>Confirm Email</a>
                        </div>
                        <p>If you did not create this account, please ignore this email.</p>
                        <p>Thank you,<br>Your Support Team</p>
                    </div>
                </div>";

                await _emailService.SendEmailAsync(dto.Email, "Confirm your email", body);
                await _UserDAO.CommitTransactionAsync();
                _logger.LogInformation("User registered successfully with email: {Email}", dto.Email);
                return user;
            }
            catch (Exception ex)
            {
                await _UserDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Failed to register user with email: {Email}", dto.Email);
                return null;
            }
        }

        public async Task<User> ConfirmAccountAsync(Guid UserId)
        {
            await _UserDAO.BeginTransactionAsync();
            try
            {
                var user = await _UserDAO.GetByIdAsync(UserId);
                if (user == null)
                {
                    _logger.LogWarning($"User with ID {UserId} not found.");
                    return null;
                }

                user.IsConfirmed = true;

                await _UserDAO.UpdateAsync(user);
                await _UserDAO.CommitTransactionAsync();
                return user;
            }
            catch (Exception ex)
            {
                await _UserDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Failed to Confirm user with email: {Email}");
                return null;
            }
        }

        public async Task ForgotPasswordAsync(ForgotpasswordDto dto)
        {
            var user = _UserDAO.GetUserbyEmail(dto.Email);
            if (user == null)
                throw new Exception("Email Not Found");
            var token = GenerateResetPasswordToken(dto.Email);
            var resetLink = $"{_clientSettings.BaseUrl}ResetPassword?Token={token}";
            string body = $@"
            <div style='font-family: Arial, sans-serif; padding: 20px; background-color: #f4f8fb; color: #333;'>
                <div style='max-width: 600px; margin: auto; background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'>
                    <h2 style='color: #007bff;'>Reset Your Password</h2>
                    <p>Hello,</p>
                    <p>You requested to reset your password. Please click the button below to proceed. This link will expire in <strong>15 minutes</strong>.</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetLink}' style='display: inline-block; background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-size: 16px;'>Reset Password</a>
                    </div>
                    <p>If you did not request a password reset, please ignore this email.</p>
                    <p>Thank you,<br>Your Support Team</p>
                </div>
            </div>";

            await _emailService.SendEmailAsync(dto.Email, "Password reset request", body);
        }

        public async Task<LoginResultDto> HandleGoogleLoginAsync(string code)
        {
            var clientId = _googlesetting.ClientId;
            var clientSecret = _googlesetting.ClientSecret;
            var redirectUri = _googlesetting.RedirectUri;

            var httpClient = new HttpClient();

            // 1. Get access_token from Google
            var tokenResponse = await httpClient.PostAsync("https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
            {"code", code},
            {"client_id", clientId},
            {"client_secret", clientSecret},
            {"redirect_uri", redirectUri},
            {"grant_type", "authorization_code"}
                }));

            var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
            var tokenJson = JsonSerializer.Deserialize<JsonElement>(tokenContent);

            var accessToken = tokenJson.GetProperty("access_token").GetString();

            // 2. Get user info from Google
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var userInfoJson = await httpClient.GetStringAsync("https://www.googleapis.com/oauth2/v2/userinfo");
            var userInfo = JsonSerializer.Deserialize<JsonElement>(userInfoJson);

            string email = userInfo.GetProperty("email").GetString();
            var fullName = userInfo.GetProperty("name").GetString();
            var nameParts = fullName?.Trim().Split(' ', 2);
            string firstName = nameParts?.Length == 2 ? nameParts[0] : fullName;
            string lastName = nameParts?.Length == 2 ? nameParts[1] : "";
            string avatar = userInfo.GetProperty("picture").GetString();

            // 3. Check if user exists
            var existingUser = _UserDAO.GetUserbyEmail(email);
            if (existingUser == null)
            {
                var newUser = new User
                {
                    UserId = Guid.NewGuid(),
                    Email = email,
                    Phone = "0000000000",
                    Password = Guid.NewGuid().ToString(),
                    IsConfirmed = true,
                    IsPremium = false,
                    IsBanned = false,
                    CreateAt = DateTime.UtcNow
                };
                await _UserDAO.CreateUserAsync(newUser);
                var metadata = new UserMetadata
                {
                    UserId = newUser.UserId,
                    FirstName = firstName,
                    LastName = lastName,
                    Avatar = avatar,
                    Gender = "Other",
                };
                await _UserDAO.AddUserMetaData(metadata);

                var defaultRole = await _UserDAO.GetRoleByNameAsync("User");
                if (defaultRole != null)
                {
                    var userRole = new UserRole
                    {
                        UserId = newUser.UserId,
                        RoleId = defaultRole.RoleId
                    };
                    await _UserDAO.AddUserRoleAsync(userRole);
                }

                existingUser = newUser;
            }

            // 4. Generate token
            if (existingUser.IsBanned)
            {
                return new LoginResultDto
                {
                    Success = false,
                    Token = null,
                    ErrorMessage = "Your account has been banned."
                };
            }
            var token = GenerateToken(existingUser);

            return new LoginResultDto
            {
                Success = true,
                Token = token
            };
        }



        public string ValidateResetToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]);

            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidAudience = _configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                throw new Exception("Invalid token");

            return email;
        }


        private string GenerateToken(User user)
        {
            // Define JWT claims
            var roles = _UserDAO.GetRolesByUserId(user.UserId);

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

        private string GenerateResetPasswordToken(string email)
        {
            var claims = new List<Claim>
            {
        new Claim(ClaimTypes.Email, email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15), // thời hạn 15 phút
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

         public async Task ResetPasswordAsync(string Token,ResetPasswordDto Request)
            {
                var email = ValidateResetToken(Token);
                var user = _UserDAO.GetUserbyEmail(email);
                if (user == null)
                    throw new Exception("User not found");

                user.Password = new PasswordHasher<User>().HashPassword(user, Request.NewPassword); // or your own method
                await _UserDAO.UpdateAsync(user);
            }

    }
}