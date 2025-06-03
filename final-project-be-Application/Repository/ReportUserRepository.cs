using AutoMapper;
using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.DTOs;
using final_project_be_Application.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace final_project_be_Application.Repository
{
    public class ReportUserRepository : Repository<ReportUser>, IReportUserRepository
    {
        private readonly ReportUserDAO _ReportUserDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportUserRepository> _logger;
        private readonly ReportDAO _reportDAO;

        public ReportUserRepository(ReportUserDAO ReportUserDAO, IMapper mapper, ILogger<ReportUserRepository> logger, ReportDAO reportDAO) : base(ReportUserDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _ReportUserDAO = ReportUserDAO;
            _reportDAO = reportDAO;
        }

        public async Task<ReportUser> CreateReportUser(ReportUserDto dto)
        {
            try
            {
                await _ReportUserDAO.BeginTransactionAsync();
                var report = _mapper.Map<Report>(dto);
                await _reportDAO.AddAsync(report);

                if (report == null || report.ReportId <= 0)
                {
                    _logger.LogError("Failed to create Report, cannot proceed with ReportComment.");
					await _ReportUserDAO.RollbackTransactionAsync();
                    return null;
                }

                dto.ReportId = report.ReportId;
                var Reportuser = _mapper.Map<ReportUser>(dto);
                await _ReportUserDAO.AddAsync(Reportuser);
				await _ReportUserDAO.CommitTransactionAsync();

                _logger.LogInformation("AddAsync ReportComment success");
                return Reportuser;
            }
            catch (Exception ex)
            {
                await _ReportUserDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding ReportComment");
                return null;
            }
        }



        public async Task<bool> DeleteReportUser(int reportId, Guid userid)
        {
            try
            {
				await _ReportUserDAO.BeginTransactionAsync();
                _ReportUserDAO.DeleteByReportAndUserId(reportId, userid);
                await _ReportUserDAO.CommitTransactionAsync();

                _logger.LogInformation("DeleteAsync ReportComment success");
                return true;
            }
            catch (Exception ex)
            {
                await _ReportUserDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when delete ReportComment");
                return false;
            }
        }

        public PageResult<ReportUser> GetAllReportUsers(int page, int pageSize)
        {
            try
            {
                var totalCount = _ReportUserDAO.GetAll().Count();
                var ReportUsers = _ReportUserDAO.GetAll()
                    .Include(x => x.Report)
                    .Include(c => c.User)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get ReportComments success");

                return new PageResult<ReportUser>(ReportUsers, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting ReportComments");
                return new PageResult<ReportUser>(new List<ReportUser>(), 0, page, pageSize);
            }
        }



        public async Task<ReportUser> GetReportUser(int id)
        {
            try
            {
				await _ReportUserDAO.BeginTransactionAsync();
                var ReportUser = _ReportUserDAO.GetByReportId(id);
                await _ReportUserDAO.CommitTransactionAsync();

                _logger.LogInformation("Get ReportComment success");
                return ReportUser;
            }
            catch (Exception ex)
            {
                await _ReportUserDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when get ReportComment");
                return null;
            }
        }



        public async Task<ReportUser> UpdateReportUser(ReportUserDto dto)
        {
            try
            {
                await _ReportUserDAO.BeginTransactionAsync();
                var report = _mapper.Map<Report>(dto);
                await _reportDAO.UpdateAsync(report);

                if (report == null || report.ReportId <= 0)
                {
                    _logger.LogError("Failed to create Report, cannot proceed with ReportComment.");
					await _ReportUserDAO.RollbackTransactionAsync();
                    return null;
                }

                dto.ReportId = report.ReportId;
                var ReportUser = _mapper.Map<ReportUser>(dto);
				await _ReportUserDAO.UpdateAsync(ReportUser);
                await _ReportUserDAO.CommitTransactionAsync();

                _logger.LogInformation("UpdateAsync ReportComment success");
                return ReportUser;
            }
            catch (Exception ex)
            {
                await _ReportUserDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when UpdateAsync ReportComment");
                return null;
            }
        }

        public PageResult<GroupedReportDto<Guid, ReportUser>> GetGroupedReportUsers(int page, int pageSize)
        {
            var all = _ReportUserDAO.GetAll()
                .Include(ru => ru.Report)
                .Include(ru => ru.User)
                .AsEnumerable();

            var grouped = all
                .GroupBy(ru => ru.UserId)
                .Select(group => new GroupedReportDto<Guid, ReportUser>
                {
                    Id = group.Key,
                    ReportCount = group.Count(),
                    Reports = group.ToList()
                })
                .OrderByDescending(g => g.ReportCount)
                .ToList();

            var totalCount = grouped.Count;
            var paged = grouped.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new PageResult<GroupedReportDto<Guid, ReportUser>>(paged, totalCount, page, pageSize);
        }
    }
}
