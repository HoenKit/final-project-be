using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;

namespace final_project_be_Infrastructure.DAO
{
    public class ReportCommentDAO : GenericDAO<ReportComment>
    {
        private readonly ApplicationDbContext _context;
        public ReportCommentDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public List<ReportComment> GetByCommentId(int commentId)
        {
            return _context.reportComments
                .Where(rp => rp.CommentId == commentId).ToList();
        }
        public ReportComment GetByReportId(int id) => _context.reportComments.Where(r => r.ReportId == id).FirstOrDefault();
        public void DeleteByReportAndCommentId(int reportId, int commentId)
        {
            var reportComments = _context.reportComments
                .Where(r => r.ReportId == reportId && r.CommentId == commentId)
                .ToList();

            if (reportComments.Any()) 
            {
                _context.RemoveRange(reportComments);
                _context.SaveChanges();
            }
        }

    }
}
