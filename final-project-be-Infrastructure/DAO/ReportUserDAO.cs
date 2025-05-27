using final_project_be_Domain.Models;
using final_project_be_Infrastructure.Data;

namespace final_project_be_Infrastructure.DAO
{
    public class ReportUserDAO : GenericDAO<ReportUser>
    {
        private readonly ApplicationDbContext _context;
        public ReportUserDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public ReportUser GetByReportId(int id) => _context.reportUser.Where(r => r.ReportId == id).FirstOrDefault();
        public void DeleteByReportAndUserId(int reportId, Guid userId)
        {
            var reportUsers = _context.reportUser
                .Where(r => r.ReportId == reportId && r.UserId == userId)
                .ToList();

            if (reportUsers.Any())
            {
                _context.RemoveRange(reportUsers);
                _context.SaveChanges();
            }
        }

    }
}
