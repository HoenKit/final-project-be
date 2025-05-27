using final_project_be_Domain.Models;
using final_project_be_Infrastructure.Data;

namespace final_project_be_Infrastructure.DAO
{
    public class ReportPostDAO : GenericDAO<ReportPost>
    {
        private readonly ApplicationDbContext _context;
        public ReportPostDAO(ApplicationDbContext context) : base(context) 
        {
            _context = context;
        }
        public ReportPost GetByReportId(int id) => _context.reportPost.Where(r => r.ReportId == id).FirstOrDefault();
        public void DeleteByReportAndPostId(int reportId, int PostId)
        {
            var reportPosts = _context.reportPost
                .Where(r => r.ReportId == reportId && r.PostId == PostId)
                .ToList();

            if (reportPosts.Any())
            {
                _context.RemoveRange(reportPosts);
                _context.SaveChanges();
            }
        }
    }
}
