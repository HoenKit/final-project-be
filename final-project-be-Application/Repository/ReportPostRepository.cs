using System.ComponentModel.Design;
using AutoMapper;
using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Application.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace final_project_be_Application.Repository
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
				await _ReportPostDAO.BeginTransactionAsync();

				// Tạo Report trước
				var report = _mapper.Map<Report>(dto);
				await _reportDAO.AddAsync(report);

				if (report == null || report.ReportId <= 0)
				{
					_logger.LogError("Failed to create Report, cannot proceed with ReportPost.");
					await _ReportPostDAO.RollbackTransactionAsync();
					return null;
				}

				// Gán ReportId vừa tạo vào dto để map sang ReportPost
				dto.ReportId = report.ReportId;

				var reportPost = _mapper.Map<ReportPost>(dto);
				await _ReportPostDAO.AddAsync(reportPost);

				await _ReportPostDAO.CommitTransactionAsync();

				_logger.LogInformation("Successfully added ReportPost with ID: {Id}", reportPost.ReportId);
				return reportPost;
			}
			catch (Exception ex)
			{
				await _ReportPostDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when adding ReportPost");
				return null;
			}
		}


		public async Task<bool> DeleteReportPost(int reportId, int PostId)
        {
            try
            {
                await _ReportPostDAO.BeginTransactionAsync();
                _ReportPostDAO.DeleteByReportAndPostId(reportId, PostId);
                await _ReportPostDAO.CommitTransactionAsync();

                _logger.LogInformation("DeleteAsync ReportPost success");
                return true;
            }
            catch (Exception ex)
            {
				await _ReportPostDAO.RollbackTransactionAsync();
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
                await _ReportPostDAO.BeginTransactionAsync();
                var ReportPost = _ReportPostDAO.GetByReportId(id);
				await _ReportPostDAO.CommitTransactionAsync();

                _logger.LogInformation("Get ReportPost success");
                return ReportPost;
            }
            catch (Exception ex)
            {
                await _ReportPostDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when get ReportPost");
                return null;
            }
        }

        public async Task<ReportPost> UpdateReportPost(ReportPostDto dto)
        {
            try
            {
				await _ReportPostDAO.BeginTransactionAsync();
                var report = _mapper.Map<Report>(dto);
				await _reportDAO.UpdateAsync(report);

                if (report == null || report.ReportId <= 0)
                {
                    _logger.LogError("Failed to create Report, cannot proceed with ReportPost.");
                    await _ReportPostDAO.RollbackTransactionAsync();
                    return null;
                }

                dto.ReportId = report.ReportId;
                var ReportPost = _mapper.Map<ReportPost>(dto);
                await _ReportPostDAO.UpdateAsync(ReportPost);
                await _ReportPostDAO.CommitTransactionAsync();

                _logger.LogInformation("UpdateAsync ReportPost success");
                return ReportPost;
            }
            catch (Exception ex)
            {
                await _ReportPostDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when UpdateAsync ReportPost");
                return null;
            }
        }

        public PageResult<GroupedReportDto<int, ReportPost>> GetGroupedReportPosts(int page, int pageSize)
        {
            var allReportPosts = _ReportPostDAO.GetAll()
                .Include(rp => rp.Report)
                .Include(rp => rp.Post)
                .AsEnumerable();

            var grouped = allReportPosts
                .GroupBy(rp => rp.PostId)
                .Select(group => new GroupedReportDto<int, ReportPost>
                {
                    Id = group.Key,
                    ReportCount = group.Count(),
                    Reports = group.ToList()
                })
                .OrderByDescending(g => g.ReportCount)
                .ToList();

            var totalCount = grouped.Count;
            var paged = grouped.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PageResult<GroupedReportDto<int, ReportPost>>(paged, totalCount, page, pageSize);
        }

        public async Task<bool> DeleteReportsByPostId(int postId)
        {
            try
            {
                await _reportDAO.BeginTransactionAsync();

                var reportPosts = _ReportPostDAO.GetByPostId(postId);

                if (reportPosts == null || !reportPosts.Any())
                {
                    _logger.LogWarning($"No reports found for postId: {postId}");
                    await _reportDAO.CommitTransactionAsync(); // vẫn commit để tránh giữ transaction
                    return false;
                }

                // 2. Lấy danh sách reportId
                var reportIds = reportPosts.Select(rp => rp.ReportId).Distinct().ToList();

                // 3. Xóa các bản ghi ReportPost
                foreach (var rp in reportPosts)
                {
                    _ReportPostDAO.DeleteByReportAndPostId(rp.ReportId, rp.PostId);
                }

                // 4. Xóa các bản ghi Report
                foreach (var reportId in reportIds)
                {
                    await _reportDAO.DeleteAsync(reportId);
                }

                await _reportDAO.CommitTransactionAsync();

                _logger.LogInformation($"Successfully deleted all reports for postId: {postId}");
                return true;
            }
            catch (Exception ex)
            {
                await _reportDAO.RollbackTransactionAsync();
                _logger.LogError(ex, $"Error deleting reports for postId: {postId}");
                return false;
            }
        }

    }
}
