using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.DTOs;

namespace final_project_be_Application.Interface
{
    public interface IReportPostRepository : IRepository<ReportPost>
    {
        public Task<ReportPost> CreateReportPost(ReportPostDto dto);
        public Task<ReportPost> GetReportPost(int id);
        public PageResult<ReportPost> GetAllReportPosts(int page, int pageSize);
        public PageResult<GroupedReportDto<int, ReportPost>> GetGroupedReportPosts(int page, int pageSize);
        public Task<bool> DeleteReportsByPostId(int postId);
    }
}
