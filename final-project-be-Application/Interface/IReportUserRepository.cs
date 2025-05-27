using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.DTOs;

namespace final_project_be_Application.Interface
{
    public interface IReportUserRepository : IRepository<ReportUser>
    {
        public Task<ReportUser> CreateReportUser(ReportUserDto dto);
        public bool DeleteReportUser(int reportId ,Guid userid);
        public Task<ReportUser> GetReportUser(int id);
        public Task<ReportUser> UpdateReportUser(ReportUserDto dto);
        public PageResult<ReportUser> GetAllReportUsers(int page, int pageSize);
    }
}
