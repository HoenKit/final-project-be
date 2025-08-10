using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using final_project_be_Infrastructure.DAO;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Application.Repository
{
    public class ReportWorkShopRepository : Repository<ReportWorkShop>, IReportWorkShopRepository
	{
		private readonly IReportWorkShopDAO _ReportWorkShopDAO;
		private readonly IMapper _mapper;
		private readonly ILogger<ReportWorkShopRepository> _logger;
		private readonly IReportDAO _reportDAO;
		public ReportWorkShopRepository(IReportWorkShopDAO ReportWorkShopDAO, IMapper mapper, ILogger<ReportWorkShopRepository> logger, IReportDAO reportDAO) : base(ReportWorkShopDAO)
		{
			_ReportWorkShopDAO = ReportWorkShopDAO;
			_mapper = mapper;
			_logger = logger;
			_reportDAO = reportDAO;
		}

		public async Task<ReportWorkShop> CreateReportWorkShop(ReportWorkShopDto dto)
		{
			try
			{
				await _ReportWorkShopDAO.BeginTransactionAsync();

				// Tạo Report trước
				var report = _mapper.Map<Report>(dto);
				await _reportDAO.AddAsync(report);

				if (report == null || report.ReportId <= 0)
				{
					_logger.LogError("Failed to create Report, cannot proceed with ReportWorkShop.");
					await _ReportWorkShopDAO.RollbackTransactionAsync();
					return null;
				}

				// Gán ReportId vừa tạo vào dto để map sang ReportWorkShop
				dto.ReportId = report.ReportId;

				var reportWorkShop = _mapper.Map<ReportWorkShop>(dto);
				await _ReportWorkShopDAO.AddAsync(reportWorkShop);

				await _ReportWorkShopDAO.CommitTransactionAsync();

				_logger.LogInformation("Successfully added ReportWorkShop with ID: {Id}", reportWorkShop.ReportId);
				return reportWorkShop;
			}
			catch (Exception ex)
			{
				await _ReportWorkShopDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when adding ReportWorkShop");
				return null;
			}
		}

		public PageResult<ReportWorkShop> GetAllReportWorkShops(int page, int pageSize)
		{
			try
			{
				var totalCount = _ReportWorkShopDAO.GetAll().Count();
				var ReportWorkShops = _ReportWorkShopDAO.GetAll()
					.Include(x => x.Report)
					.Include(c => c.WorkShop)
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToList();

				_logger.LogInformation("Get ReportWorkShops success");

				return new PageResult<ReportWorkShop>(ReportWorkShops, totalCount, page, pageSize);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when getting ReportWorkShops");
				return new PageResult<ReportWorkShop>(new List<ReportWorkShop>(), 0, page, pageSize);
			}
		}

		public async Task<ReportWorkShop> GetReportWorkShop(int id)
		{
			try
			{
				await _ReportWorkShopDAO.BeginTransactionAsync();
				var ReportWorkShop = _ReportWorkShopDAO.GetByReportId(id);
				await _ReportWorkShopDAO.CommitTransactionAsync();

				_logger.LogInformation("Get ReportWorkShop success");
				return ReportWorkShop;
			}
			catch (Exception ex)
			{
				await _ReportWorkShopDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when get ReportWorkShop");
				return null;
			}
		}
		public PageResult<GroupedReportDto<int, ReportWorkShop>> GetGroupedReportWorkShops(int page, int pageSize)
		{
			var allReportWorkShops = _ReportWorkShopDAO.GetAll()
				.Include(rp => rp.Report)
				.Include(rp => rp.WorkShop)
				.AsEnumerable();

			var grouped = allReportWorkShops
				.GroupBy(rp => rp.WorkshopId)
				.Select(group => new GroupedReportDto<int, ReportWorkShop>
				{
					Id = group.Key,
					ReportCount = group.Count(),
					Reports = group.ToList()
				})
				.OrderByDescending(g => g.ReportCount)
				.ToList();

			var totalCount = grouped.Count;
			var paged = grouped.Skip((page - 1) * pageSize).Take(pageSize).ToList();

			return new PageResult<GroupedReportDto<int, ReportWorkShop>>(paged, totalCount, page, pageSize);
		}

		public async Task<bool> DeleteReportsByWorkShopId(int workShopId)
		{
			try
			{
				await _reportDAO.BeginTransactionAsync();

				var reportWorkShops = _ReportWorkShopDAO.GetByWorkShopId(workShopId);

				if (reportWorkShops == null || !reportWorkShops.Any())
				{
					_logger.LogWarning($"No reports found for reportWorkShopId: {workShopId}");
					await _reportDAO.CommitTransactionAsync(); // vẫn commit để tránh giữ transaction
					return false;
				}

				// 2. Lấy danh sách reportId
				var reportIds = reportWorkShops.Select(rp => rp.ReportId).Distinct().ToList();

				// 3. Xóa các bản ghi ReportWorkShop
				foreach (var rp in reportWorkShops)
				{
					_ReportWorkShopDAO.DeleteByReportAndWorkShopId(rp.ReportId, rp.WorkshopId);
				}

				// 4. Xóa các bản ghi Report
				foreach (var reportId in reportIds)
				{
					await _reportDAO.DeleteAsync(reportId);
				}

				await _reportDAO.CommitTransactionAsync();

				_logger.LogInformation($"Successfully deleted all reports for workShopId: {workShopId}");
				return true;
			}
			catch (Exception ex)
			{
				await _reportDAO.RollbackTransactionAsync();
				_logger.LogError(ex, $"Error deleting reports for workShopId: {workShopId}");
				return false;
			}
		}
	}
}
