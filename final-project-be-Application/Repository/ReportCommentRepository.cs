using AutoMapper;
using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Report;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace final_project_be_Application.Repository
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
				await _ReportCommentDAO.BeginTransactionAsync();
                var report = _mapper.Map<Report>(dto);
                await _reportDAO.AddAsync(report);

                if (report == null || report.ReportId <= 0)
                {
                    _logger.LogError("Failed to create Report, cannot proceed with ReportComment.");
                    await _ReportCommentDAO.RollbackTransactionAsync();
                    return null;
                }

                dto.ReportId = report.ReportId;
                var ReportComment = _mapper.Map<ReportComment>(dto);
				await _ReportCommentDAO.AddAsync(ReportComment);
                await _ReportCommentDAO.CommitTransactionAsync();

                _logger.LogInformation("AddAsync ReportComment success");
                return ReportComment;
            }
            catch (Exception ex)
            {
                await _ReportCommentDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding ReportComment");
                return null;
            }
        }

        public async Task<bool> DeleteReportComment(int reportId, int commentId)
        {
            try
            {
				await _ReportCommentDAO.BeginTransactionAsync();
				_ReportCommentDAO.DeleteByReportAndCommentId(reportId, commentId);
				await _ReportCommentDAO.CommitTransactionAsync();

                _logger.LogInformation("DeleteAsync ReportComment success");
                return true;
            }
            catch (Exception ex)
            {
				await _ReportCommentDAO.RollbackTransactionAsync();
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
                await _ReportCommentDAO.BeginTransactionAsync();
                var ReportComment = _ReportCommentDAO.GetByReportId(id);
				await _ReportCommentDAO.CommitTransactionAsync();

                _logger.LogInformation("Get ReportComment success");
                return ReportComment;
            }
            catch (Exception ex)
            {
                await _ReportCommentDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when get ReportComment");
                return null;
            }

        }

        public async Task<ReportComment> UpdateReportComment(ReportCommentDto dto)
        {
            try
            {
				await _ReportCommentDAO.BeginTransactionAsync();
                var report = _mapper.Map<Report>(dto);
                await _reportDAO.UpdateAsync(report);

                if (report == null || report.ReportId <= 0)
                {
                    _logger.LogError("Failed to create Report, cannot proceed with ReportComment.");
                    await _ReportCommentDAO.RollbackTransactionAsync();
                    return null;
                }

                dto.ReportId = report.ReportId;
                var ReportComment = _mapper.Map<ReportComment>(dto);
				await _ReportCommentDAO.UpdateAsync(ReportComment);
                await _ReportCommentDAO.CommitTransactionAsync();

                _logger.LogInformation("UpdateAsync ReportComment success");
                return ReportComment;
            }
            catch (Exception ex)
            {
                await _ReportCommentDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when UpdateAsync ReportComment");
                return null;
            }
        }

        public PageResult<GroupedReportDto<int, ReportComment>> GetGroupedReportComments(int page, int pageSize)
        {
            var allReportPosts = _ReportCommentDAO.GetAll()
                .Include(rp => rp.Report)
                .Include(rp => rp.Comment)
                .AsEnumerable();

            var grouped = allReportPosts
                .GroupBy(rp => rp.CommentId)
                .Select(group => new GroupedReportDto<int, ReportComment>
                {
                    Id = group.Key,
                    ReportCount = group.Count(),
                    Reports = group.ToList()
                })
                .OrderByDescending(g => g.ReportCount)
                .ToList();

            var totalCount = grouped.Count;
            var paged = grouped.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PageResult<GroupedReportDto<int, ReportComment>>(paged, totalCount, page, pageSize);
        }
    }
}
