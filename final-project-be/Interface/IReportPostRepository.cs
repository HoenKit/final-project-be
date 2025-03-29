using final_project_be.Data.Models;
using final_project_be.Dtos.Report;
using final_project_be.Dtos;

namespace final_project_be.Interface
{
    public interface IReportPostRepository : IRepository<ReportPost>
    {
        public Task<ReportPost> CreateReportPost(ReportPostDto dto);
        public bool DeleteReportPost(int reportId, int PostId);
        public Task<ReportPost> GetReportPost(int id);
        public Task<ReportPost> UpdateReportPost(ReportPostDto dto);
        public PageResult<ReportPost> GetAllReportPosts(int page, int pageSize);
    }
}
