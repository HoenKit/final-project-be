using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos.Report;
using final_project_be.Dtos;
using final_project_be.Interface;

namespace final_project_be.Repository
{
    public class ReportRepository : Repository<Report>, IReportRepository
    {
        private readonly ReportDAO _ReportDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<ReportRepository> _logger;

        public ReportRepository(ReportDAO ReportDAO, IMapper mapper, ILogger<ReportRepository> logger) : base(ReportDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _ReportDAO = ReportDAO;
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

        public PageResult<Report> GetAllReports(int page, int pageSize)
        {
            try
            {
                var totalCount = _ReportDAO.GetAll().Count();
                var Reports = _ReportDAO.GetAll()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get Reports success");

                return new PageResult<Report>(Reports, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting Reports");
                return new PageResult<Report>(new List<Report>(), 0, page, pageSize);
            }
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
