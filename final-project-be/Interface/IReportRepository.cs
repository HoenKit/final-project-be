using final_project_be.Data.Models;
using final_project_be.Dtos.Report;
using final_project_be.Dtos;

namespace final_project_be.Interface
{
    public interface IReportRepository : IRepository<Report>
    {
        public Report CreateReport(ReportDto dto);
        public bool DeleteReport(int id);
        public Task<Report> GetReport(int id);
        public Task<Report> UpdateReport(ReportDto dto);
        public PageResult<Report> GetAllReports(int page, int pageSize);
    }
}
