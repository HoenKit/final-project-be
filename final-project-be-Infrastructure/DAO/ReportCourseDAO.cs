using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class ReportCourseDAO : GenericDAO<ReportCourse>, IReportCourseDAO
    {
        private readonly ApplicationDbContext _context;

        public ReportCourseDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public List<ReportCourse> GetByCourseId(int courseId)
        {
            return _context.ReportCourse
                .Where(rp => rp.CourseId == courseId)
                .ToList();
        }

        public ReportCourse GetByReportId(int id)
        {
            return _context.ReportCourse
                .FirstOrDefault(r => r.ReportId == id);
        }

        public void DeleteByReportAndCourseId(int reportId, int courseId)
        {
            var reportCourses = _context.ReportCourse
                .Where(r => r.ReportId == reportId && r.CourseId == courseId)
                .ToList();

            if (reportCourses.Any())
            {
                _context.RemoveRange(reportCourses);
                _context.SaveChanges();
            }
        }
    }

}
