using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class ReportPostDAO : GenericDAO<ReportPost>, IReportPostDAO
    {
        private readonly ApplicationDbContext _context;

        public ReportPostDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public List<ReportPost> GetByPostId(int postId)
        {
            return _context.reportPost
                .Where(rp => rp.PostId == postId)
                .ToList();
        }

        public ReportPost GetByReportId(int id)
        {
            return _context.reportPost
                .FirstOrDefault(r => r.ReportId == id);
        }

        public void DeleteByReportAndPostId(int reportId, int postId)
        {
            var reportPosts = _context.reportPost
                .Where(r => r.ReportId == reportId && r.PostId == postId)
                .ToList();

            if (reportPosts.Any())
            {
                _context.RemoveRange(reportPosts);
                _context.SaveChanges();
            }
        }
    }

}
