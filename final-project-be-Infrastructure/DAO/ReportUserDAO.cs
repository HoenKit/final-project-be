using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;

namespace final_project_be_Infrastructure.DAO
{
    public class ReportUserDAO : GenericDAO<ReportUser>, IReportUserDAO
    {
        private readonly ApplicationDbContext _context;

        public ReportUserDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public List<ReportUser> GetByUserId(Guid userId)
        {
            return _context.reportUser
                .Where(rp => rp.UserId == userId)
                .ToList();
        }

        public ReportUser GetByReportId(int id)
        {
            return _context.reportUser
                .FirstOrDefault(r => r.ReportId == id);
        }

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
