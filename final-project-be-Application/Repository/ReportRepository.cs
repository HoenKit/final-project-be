using AutoMapper;
using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.DTOs;
using final_project_be_Application.Interface;
using Microsoft.Extensions.Logging;

namespace final_project_be_Application.Repository
{
    public class ReportRepository : Repository<Report>, IReportRepository
    {
        private readonly ReportDAO _ReportDAO;
        private readonly ReportPostDAO _ReportPostDAO;
        private readonly ReportUserDAO _ReportUserDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportRepository> _logger;

        public ReportRepository(ReportDAO ReportDAO, ReportPostDAO ReportPostDAO, ReportUserDAO ReportUserDAO, IMapper mapper, ILogger<ReportRepository> logger) : base(ReportDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _ReportDAO = ReportDAO;
            _ReportPostDAO = ReportPostDAO;
            _ReportUserDAO = ReportUserDAO;
        }

        public Report CreateReport(ReportDto dto)
        {
            try
            {
                _ReportDAO.BeginTransaction();
                var Report = _mapper.Map<Report>(dto);
                _ReportDAO.Add(Report);
                _ReportDAO.SaveChanges();
                _ReportDAO.CommitTransaction();

                _logger.LogInformation("Add Report success");
                return Report;
            }
            catch (Exception ex)
            {
                _ReportDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when adding Report");
                return null;
            }
        }

        public bool DeleteReport(int id)
        {
            try
            {
                _ReportDAO.BeginTransaction();
                _ReportDAO.Delete(id);
                _ReportDAO.CommitTransaction();

                _logger.LogInformation("Delete Report success");
                return true;
            }
            catch (Exception ex)
            {
                _ReportDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when delete Report");
                return false;
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

        public async Task<Report> GetReport(int id)
        {
            try
            {
                _ReportDAO.BeginTransaction();
                var Report = _ReportDAO.GetById(id);
                _ReportDAO.CommitTransaction();

                _logger.LogInformation("Get Report success");
                return Report;
            }
            catch (Exception ex)
            {
                _ReportDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get Report");
                return null;
            }

        }

        public async Task<Report> UpdateReport(ReportDto dto)
        {
            try
            {
                _ReportDAO.BeginTransaction();
                var Report = _mapper.Map<Report>(dto);
                _ReportDAO.Update(Report);
                _ReportDAO.CommitTransaction();

                _logger.LogInformation("Update Report success");
                return Report;
            }
            catch (Exception ex)
            {
                _ReportDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update Report");
                return null;
            }
        }
    }
}
