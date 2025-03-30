using final_project_be.Data.Models;
using final_project_be.Dtos.Report;
using final_project_be.Dtos;

namespace final_project_be.Interface
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
