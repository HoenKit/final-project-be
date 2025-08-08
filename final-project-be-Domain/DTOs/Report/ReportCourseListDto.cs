using final_project_be_Domain.DTOs.Courses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Report
{
    public class ReportCourseListDto
    {
        public ReportDto ReportDto { get; set; }
        public CourseDto CourseDto { get; set; }
    }
}
