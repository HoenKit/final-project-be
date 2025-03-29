using System.ComponentModel.Design;
using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.Report;
using final_project_be.Interface;
using Microsoft.EntityFrameworkCore;

namespace final_project_be.Repository
{
    public class ReportPostRepository : Repository<ReportPost>, IReportPostRepository
    {
        private readonly ReportPostDAO _ReportPostDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportPostRepository> _logger;
        private readonly ReportDAO _reportDAO;
        public ReportPostRepository(ReportPostDAO ReportPostDAO, IMapper mapper, ILogger<ReportPostRepository> logger, ReportDAO reportDAO) : base(ReportPostDAO)
        {
            _ReportPostDAO = ReportPostDAO;
            _mapper = mapper;
            _logger = logger;
            _reportDAO = reportDAO;
        }

        public async Task<ReportPost> CreateReportPost(ReportPostDto dto)
        {
            try
            {
                _ReportPostDAO.BeginTransaction();
                var report = _mapper.Map<Report>(dto);
                _reportDAO.Add(report);

                if (report == null || report.ReportId <= 0)
                {
                    _logger.LogError("Failed to create Report, cannot proceed with ReportPost.");
                    _ReportPostDAO.RollbackTransaction();
                    return null;
                }

                dto.ReportId = report.ReportId;
                var ReportPost = _mapper.Map<ReportPost>(dto);
                _ReportPostDAO.Add(ReportPost);
                _ReportPostDAO.CommitTransaction();

                _logger.LogInformation("Add ReportPost success");
                return ReportPost;
            }
            catch (Exception ex)
            {
                _ReportPostDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when adding ReportPost");
                return null;
            }
        }

        public bool DeleteReportPost(int reportId, int PostId)
        {
            try
            {
                _ReportPostDAO.BeginTransaction();
                _ReportPostDAO.DeleteByReportAndPostId(reportId, PostId);
                _ReportPostDAO.CommitTransaction();

                _logger.LogInformation("Delete ReportPost success");
                return true;
            }
            catch (Exception ex)
            {
                _ReportPostDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when delete ReportPost");
                return false;
            }
        }

        public PageResult<ReportPost> GetAllReportPosts(int page, int pageSize)
        {
            try
            {
                var totalCount = _ReportPostDAO.GetAll().Count();
                var ReportPosts = _ReportPostDAO.GetAll()
                    .Include(x => x.Report)
                    .Include(c => c.Post)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get ReportPosts success");

                return new PageResult<ReportPost>(ReportPosts, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting ReportPosts");
                return new PageResult<ReportPost>(new List<ReportPost>(), 0, page, pageSize);
            }
        }

        public async Task<ReportPost> GetReportPost(int id)
        {
            try
            {
                _ReportPostDAO.BeginTransaction();
                var ReportPost = _ReportPostDAO.GetByReportId(id);
                _ReportPostDAO.CommitTransaction();

                _logger.LogInformation("Get ReportPost success");
                return ReportPost;
            }
            catch (Exception ex)
            {
                _ReportPostDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get ReportPost");
                return null;
            }
        }

        public async Task<ReportPost> UpdateReportPost(ReportPostDto dto)
        {
            try
            {
                _ReportPostDAO.BeginTransaction();
                var report = _mapper.Map<Report>(dto);
                _reportDAO.Update(report);

                if (report == null || report.ReportId <= 0)
                {
                    _logger.LogError("Failed to create Report, cannot proceed with ReportPost.");
                    _ReportPostDAO.RollbackTransaction();
                    return null;
                }

                dto.ReportId = report.ReportId;
                var ReportPost = _mapper.Map<ReportPost>(dto);
                _ReportPostDAO.Update(ReportPost);
                _ReportPostDAO.CommitTransaction();

                _logger.LogInformation("Update ReportPost success");
                return ReportPost;
            }
            catch (Exception ex)
            {
                _ReportPostDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update ReportPost");
                return null;
            }
        }
    }
}
