using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using final_project_be_Application.Interface;
using final_project_be_Application.Service.EmailService;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Module;
using final_project_be_Domain.DTOs.Notification;
using final_project_be_Domain.DTOs.Post;
using final_project_be_Domain.DTOs.Review;
using final_project_be_Domain.DTOs.Users;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{
    public class MentorRepository : Repository<Mentor>, IMentorRepository
    {
        private readonly UserDAO _userDAO;
        private readonly MentorDAO _mentorDAO;
        private readonly ReviewDAO _reviewDAO;
        private readonly CourseDAO _courseDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<MentorRepository> _logger;
        private readonly IEmailService _emailService;
        private readonly BlobStorageService _blobStorageService;

        public MentorRepository(MentorDAO mentorDAO, BlobStorageService blobStorageService, IMapper mapper, ILogger<MentorRepository> logger, CourseDAO courseDAO, ReviewDAO reviewDAO, UserDAO userDAO, IEmailService emailService) : base(mentorDAO)
        {
            _mentorDAO = mentorDAO;
            _mapper = mapper;
            _logger = logger;
            _courseDAO = courseDAO;
            _reviewDAO = reviewDAO;
            _userDAO = userDAO;
            _emailService = emailService;
            _blobStorageService= blobStorageService;
        }


        public async Task<Mentor> CreateMentor(CreateMentorDto dto)
        {
            try
            {
                await _mentorDAO.BeginTransactionAsync();
                var mentor = _mapper.Map<Mentor>(dto);
                if (dto.Signature != null && dto.Signature.Length > 0)
                {

                    // Generate a unique filename using GUID
                    var fileExtension = Path.GetExtension(dto.Signature.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    // Upload to Azure Blob Storage
                    using (var stream = dto.Signature.OpenReadStream())
                    {
                        await _blobStorageService.UploadFileAsync(uniqueFileName, stream);
                    }

                    // Store the full URL in the database (UpdateAsync the blob URL with your storage account name)
                    mentor.Signature = $"https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/{uniqueFileName}";
                }
                await _mentorDAO.AddAsync(mentor);
                await _mentorDAO.SaveChangesAsync();

                var user = await _userDAO.GetUserMetadatabyId(dto.UserId);
                if (user == null)
                    throw new Exception("User not found");
                if (user == null)
                {
                    user = new UserMetadata
                    {
                        FirstName = dto.FirstName,
                        LastName = dto.LastName
                    };
                }
                else
                {
                    user.FirstName = dto.FirstName;
                    user.LastName = dto.LastName;
                }

                await _userDAO.UpdateUserMetadataAsync(user);


                var mentorRole = await _userDAO.GetRoleByNameAsync("Mentor");
                if (mentorRole == null)
                {
                    mentorRole = new Role { RoleName = "Mentor" };
                    await _userDAO.AddRoleAsync(mentorRole);
                    await _userDAO.SaveChangesAsync();
                }
                var userRoleExists = await _userDAO.ExistsAsync(dto.UserId, mentorRole.RoleId);
                if (!userRoleExists)
                {
                    var userRole = new UserRole
                    {
                        UserId = dto.UserId,
                        RoleId = mentorRole.RoleId
                    };
                    await _userDAO.AddUserRoleAsync(userRole);
                    await _userDAO.SaveChangesAsync();
                }

                string body = $@"
                <div style='font-family: Arial, sans-serif; padding: 20px; background-color: #f4f8fb; color: #333;'>
                    <div style='max-width: 600px; margin: auto; background-color: #ffffff; padding: 30px; border-radius: 10px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);'>
                        <h2 style='color: #28a745;'>Welcome to the  Phronesis Learning Mentor Community!</h2>
                        <p>We are thrilled to have you join our growing network of passionate mentors who are committed to sharing knowledge and guiding learners on their journey.</p>
                        <p>Your role as a Mentor is truly meaningful — you will be making a positive impact on the personal and professional growth of many students.</p>
                        <p>Let’s work together to spread valuable knowledge and build a vibrant learning community.</p>
                        <p>If you ever have any questions or need assistance, feel free to reach out to the Phronesis Learning team.</p>
                        <p>We wish you a fulfilling and inspiring experience with Phronesis!</p>
                        <p>Warm regards,<br/>The Phronesis Learning Team</p>
                    </div>
                </div>";

                await _emailService.SendEmailAsync(user.User.Email, "Welcome to become a Mentor", body);

                await _mentorDAO.CommitTransactionAsync();
                _logger.LogInformation("CreateMentor success");
                return mentor;
            }
            catch (Exception ex)
            {
                await _mentorDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding Module");
                return null;
            }
        }

        public async Task<bool> DeleteMentor(int id)
        {
            try
            {
                await _mentorDAO.BeginTransactionAsync();
                await _mentorDAO.DeleteAsync(id);
                await _mentorDAO.CommitTransactionAsync();
                _logger.LogInformation("Delete Module success");
                return true;
            }
            catch (Exception ex)
            {
                await _mentorDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when delete Module");
                return false;
            }
        }

        public PageResult<GetMentorDto> GetAllMentors(int page, int pageSize)
        {
            try
            {
                var baseQuery = _mentorDAO.GetAll()
                    .Include(c => c.MentorCertificates)
                    .OrderByDescending(p => p.CreateAt);

                var totalCount = baseQuery.Count();
                var mentors = baseQuery
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                var mentorsDtos = mentors.Select(p => new GetMentorDto
                {
                    MentorId = p.MentorId,
                    UserId = p.UserId,
                    Introduction = p.Introduction,
                    JobTitle = p.JobTitle,
                    StudyLevel = p.StudyLevel,
                    CitizenID = p.CitizenID,
                    Signature = p.Signature,
                    IssuePlace = p.IssuePlace,
                    ExpiredDate = p.ExpiredDate,
                    IssueDate = p.IssueDate,
                    CreateAt = p.CreateAt,
                    UpdateAt = p.UpdateAt,
                    MentorCertificates = p.MentorCertificates?.Select(c => new GetMentorCertificateDto
                    {
                        MentorCertificateId = c.MentorCertificateId,
                        FileUrl = c.FileUrl,
                        CertificateName = c.CertificateName
                    }).ToList()
                }).ToList();
                _logger.LogInformation("Get Notifications success");

                return new PageResult<GetMentorDto>(mentorsDtos, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting Notifications");
                return new PageResult<GetMentorDto>(new List<GetMentorDto>(), 0, page, pageSize);
            }
        }

        public async Task<GetMentorDto> GetMentorandCertificate(int id)
        {
            try
            {
                await _mentorDAO.BeginTransactionAsync();
                var mentor = await  _mentorDAO.GetMentorandcertificate(id);
                if (mentor == null)
                    return null;
                var mentorDto = _mapper.Map<GetMentorDto>(mentor);

                var courses = _courseDAO.GetAll().Where(c => c.MentorId == id).ToList();
                var courseIds = courses.Select(c => c.CourseId).ToList();

                mentorDto.TotalCourses = courses.Count;
                mentorDto.TotalStudents = courses.Sum(c => c.StudentCount ?? 0);

                // Review data
                var reviews = _reviewDAO.GetAll()
                    .Where(r => courseIds.Contains(r.CourseId) && !r.IsDeleted)
                    .ToList();

                mentorDto.TotalReviews = reviews.Count;
                mentorDto.AverageRating = reviews.Count > 0
                    ? Math.Round(reviews.Average(r => r.Rate), 1)
                    : 0;

                await _mentorDAO.CommitTransactionAsync();

                _logger.LogInformation("Get Mentor success");
                return mentorDto;
            }
            catch (Exception ex)
            {
                await _mentorDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when get Mentor");
                return null;
            }
        }

        public async Task<GetMentorDto> GetMentorByUserId(Guid userId)
        {
            try
            {
                await _mentorDAO.BeginTransactionAsync();
                var mentor = await _mentorDAO.GetMentorByUserId(userId);
                if (mentor == null)
                    return null;
                var mentorDto = _mapper.Map<GetMentorDto>(mentor);

                var courses = _courseDAO.GetAll().Where(c => c.MentorId == mentor.MentorId).ToList();
                var courseIds = courses.Select(c => c.CourseId).ToList();

                mentorDto.TotalCourses = courses.Count;
                mentorDto.TotalStudents = courses.Sum(c => c.StudentCount ?? 0);

                // Review data
                var reviews = _reviewDAO.GetAll()
                    .Where(r => courseIds.Contains(r.CourseId) && !r.IsDeleted)
                    .ToList();

                mentorDto.TotalReviews = reviews.Count;
                mentorDto.AverageRating = reviews.Count > 0
                    ? Math.Round(reviews.Average(r => r.Rate), 1)
                    : 0;
                await _mentorDAO.CommitTransactionAsync();

                _logger.LogInformation("Get Mentor success");
                return mentorDto;
            }
            catch (Exception ex)
            {
                await _mentorDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when get Mentor");
                return null;
            }
        }

        public async Task<Mentor> UpdateMentor(CreateMentorDto dto)
        {
            try
            {
                await _mentorDAO.BeginTransactionAsync();
                var mentor = _mapper.Map<Mentor>(dto);
                await _mentorDAO.UpdateAsync(mentor);
                await _mentorDAO.CommitTransactionAsync();

                _logger.LogInformation("UpdateAsync Notification success");
                return mentor;
            }
            catch (Exception ex)
            {
                await _mentorDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when UpdateAsync Notification");
                return null;
            }
        }
    }
}
