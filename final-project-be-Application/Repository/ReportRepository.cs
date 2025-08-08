using AutoMapper;
using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.DTOs;
using final_project_be_Application.Interface;
using Microsoft.Extensions.Logging;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Application.Repository
{
    public class ReportRepository : Repository<Report>, IReportRepository
    {
        private readonly IReportDAO _ReportDAO;
        private readonly IReportPostDAO _ReportPostDAO;
        private readonly IReportUserDAO _ReportUserDAO;
        private readonly IReportCourseDAO _ReportCourseDAO;
        private readonly IReportCommentDAO _ReportCommentDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportRepository> _logger;

        public ReportRepository(IReportDAO ReportDAO, IReportPostDAO ReportPostDAO, IReportUserDAO ReportUserDAO, IReportCommentDAO ReportCommentDAO, IReportCourseDAO reportCourseDAO, IMapper mapper, ILogger<ReportRepository> logger) : base(ReportDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _ReportDAO = ReportDAO;
            _ReportPostDAO = ReportPostDAO;
            _ReportUserDAO = ReportUserDAO;
            _ReportCommentDAO = ReportCommentDAO;
            _ReportCourseDAO = reportCourseDAO;
        }

        public async Task<Report> CreateReport(ReportDto dto)
        {
            try
            {
				await _ReportDAO.BeginTransactionAsync();
                var Report = _mapper.Map<Report>(dto);
                await _ReportDAO.AddAsync(Report);
                await _ReportDAO.SaveChangesAsync();
				await _ReportDAO.CommitTransactionAsync();

                _logger.LogInformation("AddAsync Report success");
                return Report;
            }
            catch (Exception ex)
            {
                await _ReportDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding Report");
                return null;
            }
        }

        public List<ReportPostDto> GetReportsByPost(int postId)
        {
            var reportPosts = _ReportPostDAO.GetByPostId(postId);

            // Nếu cần truy vấn Report hoặc Post, có thể lấy ở đây
            var reportIds = reportPosts.Select(x => x.ReportId).ToList();
            var reports = _ReportDAO.GetAll().Where(r => reportIds.Contains(r.ReportId)).ToList();

            // Mapping và kết hợp logic (tuỳ theo bạn cần ReportDto hay PostDto gì nữa)
            var reportDtos = (from rp in reportPosts
                              join r in reports on rp.ReportId equals r.ReportId
                              select new ReportPostDto
                              {
                                  ReportId = r.ReportId,
                                  PostId = rp.PostId,
                                  UserId = r.UserId,
                                  Content = r.Content
                              }).ToList();

            return reportDtos;
        }

        public List<ReportUserDto> GetReportsByUser(Guid userId)
        {
            var reportUsers = _ReportUserDAO.GetByUserId(userId);

            // Nếu cần truy vấn Report hoặc User, có thể lấy ở đây
            var reportIds = reportUsers.Select(x => x.ReportId).ToList();
            var reports = _ReportDAO.GetAll().Where(r => reportIds.Contains(r.ReportId)).ToList();

            // Mapping và kết hợp logic (tuỳ theo bạn cần ReportDto hay UserDto gì nữa)
            var reportDtos = (from rp in reportUsers
                              join r in reports on rp.ReportId equals r.ReportId
                              select new ReportUserDto
                              {
                                  ReportId = r.ReportId,
                                  UserId = rp.UserId,
                                  UserreportedId = r.UserId,
                                  Content = r.Content
                              }).ToList();

            return reportDtos;
        }

        public List<ReportCommentDto> GetReportsByComment(int commentId)
        {
            var reportComments = _ReportCommentDAO.GetByCommentId(commentId);

            // Truy vấn Report hoặc Comment
            var reportIds = reportComments.Select(x => x.ReportId).ToList();
            var reports = _ReportDAO.GetAll().Where(r => reportIds.Contains(r.ReportId)).ToList();

            // Mapping và kết hợp logic
            var reportDtos = (from rp in reportComments
                              join r in reports on rp.ReportId equals r.ReportId
                              select new ReportCommentDto
                              {
                                  ReportId = r.ReportId,
                                  CommentId = rp.CommentId,
                                  UserId = r.UserId,
                                  Content = r.Content
                              }).ToList();

            return reportDtos;
        }

        public List<ReportCourseDto> GetReportsByCourse(int courseId)
        {
            var reportCourses = _ReportCourseDAO.GetByCourseId(courseId);

            // Truy vấn Report hoặc Course
            var reportIds = reportCourses.Select(x => x.ReportId).ToList();
            var reports = _ReportDAO.GetAll().Where(r => reportIds.Contains(r.ReportId)).ToList();

            // Mapping và kết hợp logic
            var reportDtos = (from rp in reportCourses
                              join r in reports on rp.ReportId equals r.ReportId
                              select new ReportCourseDto
                              {
                                  ReportId = r.ReportId,
                                  CourseId = rp.CourseId,
                                  UserId = r.UserId,
                                  Content = r.Content
                              }).ToList();

            return reportDtos;
        }

        public async Task<Report> GetReport(int id)
        {
            try
            {
                await _ReportDAO.BeginTransactionAsync();
                var Report = await _ReportDAO.GetByIdAsync(id);
                await _ReportDAO.CommitTransactionAsync();

                _logger.LogInformation("Get Report success");
                return Report;
            }
            catch (Exception ex)
            {
                await _ReportDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when get Report");
                return null;
            }

        }
    }
}
