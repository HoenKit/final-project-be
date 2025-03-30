using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos.Report;
using final_project_be.Dtos;
using final_project_be.Interface;
using Microsoft.EntityFrameworkCore;

namespace final_project_be.Repository
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
                _ReportUserDAO.BeginTransaction();
                var report = _mapper.Map<Report>(dto);
                _reportDAO.Add(report);

                if (report == null || report.ReportId <= 0)
                {
                    _logger.LogError("Failed to create Report, cannot proceed with ReportComment.");
                    _ReportUserDAO.RollbackTransaction();
                    return null;
                }

                dto.ReportId = report.ReportId;
                var Reportuser = _mapper.Map<ReportUser>(dto);
                _ReportUserDAO.Add(Reportuser);
                _ReportUserDAO.CommitTransaction();

                _logger.LogInformation("Add ReportComment success");
                return Reportuser;
            }
            catch (Exception ex)
            {
                _ReportUserDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when adding ReportComment");
                return null;
            }
        }



        public bool DeleteReportUser(int reportId, Guid userid)
        {
            try
            {
                _ReportUserDAO.BeginTransaction();
                _ReportUserDAO.DeleteByReportAndUserId(reportId, userid);
                _ReportUserDAO.CommitTransaction();

                _logger.LogInformation("Delete ReportComment success");
                return true;
            }
            catch (Exception ex)
            {
                _ReportUserDAO.RollbackTransaction();
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
                _ReportUserDAO.BeginTransaction();
                var ReportUser = _ReportUserDAO.GetByReportId(id);
                _ReportUserDAO.CommitTransaction();

                _logger.LogInformation("Get ReportComment success");
                return ReportUser;
            }
            catch (Exception ex)
            {
                _ReportUserDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get ReportComment");
                return null;
            }
        }



        public async Task<ReportUser> UpdateReportUser(ReportUserDto dto)
        {
            try
            {
                _ReportUserDAO.BeginTransaction();
                var report = _mapper.Map<Report>(dto);
                _reportDAO.Update(report);

                if (report == null || report.ReportId <= 0)
                {
                    _logger.LogError("Failed to create Report, cannot proceed with ReportComment.");
                    _ReportUserDAO.RollbackTransaction();
                    return null;
                }

                dto.ReportId = report.ReportId;
                var ReportUser = _mapper.Map<ReportUser>(dto);
                _ReportUserDAO.Update(ReportUser);
                _ReportUserDAO.CommitTransaction();

                _logger.LogInformation("Update ReportComment success");
                return ReportUser;
            }
            catch (Exception ex)
            {
                _ReportUserDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update ReportComment");
                return null;
            }
        }
    }
}
