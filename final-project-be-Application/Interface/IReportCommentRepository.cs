using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Report;

namespace final_project_be_Application.Interface
{
    public interface IReportCommentRepository : IRepository<ReportComment>
    {
        public Task<ReportComment> CreateReportComment(ReportCommentDto dto);
        public Task<ReportComment> GetReportComment(int id);
        public PageResult<ReportComment> GetAllReportComments(int page, int pageSize);
        public PageResult<GroupedReportDto<int, ReportComment>> GetGroupedReportComments(int page, int pageSize);
        public Task<bool> DeleteReportsByCommentId(int commentId);
    }
}
