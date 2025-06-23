using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Module;
using final_project_be_Domain.DTOs.Notification;
using final_project_be_Domain.DTOs.Post;
using final_project_be_Domain.DTOs.Review;
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
        private readonly MentorDAO _mentorDAO;
        private readonly ReviewDAO _reviewDAO;
        private readonly CourseDAO _courseDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<MentorRepository> _logger;

        public MentorRepository(MentorDAO mentorDAO, IMapper mapper, ILogger<MentorRepository> logger, CourseDAO courseDAO, ReviewDAO reviewDAO) : base(mentorDAO)
        {
            _mentorDAO = mentorDAO;
            _mapper = mapper;
            _logger = logger;
            _courseDAO = courseDAO;
            _reviewDAO = reviewDAO;
        }


        public async Task<Mentor> CreateMentor(CreateMentorDto dto)
        {
            try
            {
                await _mentorDAO.BeginTransactionAsync();
                var mentor = _mapper.Map<Mentor>(dto);
                await _mentorDAO.AddAsync(mentor);
                await _mentorDAO.CommitTransactionAsync();
                _logger.LogInformation("AddAsync Module success");
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
                    ? Math.Round(reviews.Average(r => r.rate), 1)
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
                    ? Math.Round(reviews.Average(r => r.rate), 1)
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
