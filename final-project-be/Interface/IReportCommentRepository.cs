using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.Report;

namespace final_project_be.Interface
{
    public interface IReportCommentRepository : IRepository<ReportComment>
    {
        public Task<ReportComment> CreateReportComment(ReportCommentDto dto);
        public bool DeleteReportComment(int reportId, int commentId);
        public Task<ReportComment> GetReportComment(int id);
        public Task<ReportComment> UpdateReportComment(ReportCommentDto dto);
        public PageResult<ReportComment> GetAllReportComments(int page, int pageSize);
    }
}
