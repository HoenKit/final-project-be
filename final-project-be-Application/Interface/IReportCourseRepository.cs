using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using final_project_be_Infrastructure.DAO;

namespace final_project_be_Application.Interface
{
    public interface IReportCourseRepository : IRepository<ReportCourse>
    {
        public Task<ReportCourse> CreateReportCourse(ReportCourseDto dto);
        public Task<ReportCourse> GetReportCourse(int id);
        public PageResult<ReportCourse> GetAllReportCourses(int page, int pageSize);
        public PageResult<GroupedReportDto<int, ReportCourse>> GetGroupedReportCourses(int page, int pageSize);
        public Task<bool> DeleteReportsByCourseId(int courseId);
    }
}
