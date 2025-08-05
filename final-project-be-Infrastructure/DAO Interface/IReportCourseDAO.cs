using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IReportCourseDAO : IGenericDAO<ReportCourse>
    {
        List<ReportCourse> GetByCoursedId(int courseId);
        ReportCourse GetByReportId(int id);
        void DeleteByReportAndCourseId(int reportId, int courseId);
    }
}
