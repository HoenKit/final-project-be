using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Interface;
using final_project_be.Dtos.Report;
using Microsoft.EntityFrameworkCore;

namespace final_project_be.Repository
{
    public class ReportCommentRepository : Repository<ReportComment>, IReportCommentRepository
    {
        private readonly ReportCommentDAO _ReportCommentDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportCommentRepository> _logger;
        private readonly ReportDAO _reportDAO;

        public ReportCommentRepository(ReportCommentDAO ReportCommentDAO, IMapper mapper, ILogger<ReportCommentRepository> logger, ReportDAO reportDAO) : base(ReportCommentDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _ReportCommentDAO = ReportCommentDAO;
            _reportDAO = reportDAO;
        }

        public async Task<ReportComment> CreateReportComment(ReportCommentDto dto)
        {
            try
            {
                _ReportCommentDAO.BeginTransaction();
                var report = _mapper.Map<Report>(dto);
                _reportDAO.Add(report);

                if (report == null || report.ReportId <= 0)
                {
                    _logger.LogError("Failed to create Report, cannot proceed with ReportComment.");
                    _ReportCommentDAO.RollbackTransaction();
                    return null;
                }

                dto.ReportId = report.ReportId;
                var ReportComment = _mapper.Map<ReportComment>(dto);
                _ReportCommentDAO.Add(ReportComment);
                _ReportCommentDAO.CommitTransaction();

                _logger.LogInformation("Add ReportComment success");
                return ReportComment;
            }
            catch (Exception ex)
            {
                _ReportCommentDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when adding ReportComment");
                return null;
            }
        }

        public bool DeleteReportComment(int reportId, int commentId)
        {
            try
            {
                _ReportCommentDAO.BeginTransaction();
                _ReportCommentDAO.DeleteByReportAndCommentId(reportId, commentId);
                _ReportCommentDAO.CommitTransaction();

                _logger.LogInformation("Delete ReportComment success");
                return true;
            }
            catch (Exception ex)
            {
                _ReportCommentDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when delete ReportComment");
                return false;
            }
        }

        public PageResult<ReportComment> GetAllReportComments(int page, int pageSize)
        {
            try
            {
                var totalCount = _ReportCommentDAO.GetAll().Count();
                var ReportComments = _ReportCommentDAO.GetAll()
                    .Include(x => x.Report)
                    .Include(c => c.Comment)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get ReportComments success");

                return new PageResult<ReportComment>(ReportComments, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting ReportComments");
                return new PageResult<ReportComment>(new List<ReportComment>(), 0, page, pageSize);
            }
        }

        public async Task<ReportComment> GetReportComment(int id)
        {
            try
            {
                _ReportCommentDAO.BeginTransaction();
                var ReportComment = _ReportCommentDAO.GetByReportId(id);
                _ReportCommentDAO.CommitTransaction();

                _logger.LogInformation("Get ReportComment success");
                return ReportComment;
            }
            catch (Exception ex)
            {
                _ReportCommentDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get ReportComment");
                return null;
            }

        }

        public async Task<ReportComment> UpdateReportComment(ReportCommentDto dto)
        {
            try
            {
                _ReportCommentDAO.BeginTransaction();
                var report = _mapper.Map<Report>(dto);
                _reportDAO.Update(report);

                if (report == null || report.ReportId <= 0)
                {
                    _logger.LogError("Failed to create Report, cannot proceed with ReportComment.");
                    _ReportCommentDAO.RollbackTransaction();
                    return null;
                }

                dto.ReportId = report.ReportId;
                var ReportComment = _mapper.Map<ReportComment>(dto);
                _ReportCommentDAO.Update(ReportComment);
                _ReportCommentDAO.CommitTransaction();

                _logger.LogInformation("Update ReportComment success");
                return ReportComment;
            }
            catch (Exception ex)
            {
                _ReportCommentDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update ReportComment");
                return null;
            }
        }
    }
}
