using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.DTOs;

namespace final_project_be_Application.Interface
{
    public interface IReportRepository : IRepository<Report>
    {
        public Report CreateReport(ReportDto dto);
        public bool DeleteReport(int id);
        public Task<Report> GetReport(int id);
        public Task<Report> UpdateReport(ReportDto dto);
        public List<ReportPostDto> GetReportsByPost(int postId);
        public List<ReportUserDto> GetReportsByUser(Guid userId);
    }
}
