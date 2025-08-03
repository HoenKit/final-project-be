using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Report
{
    public class ReportCourseDto
    {
        public int ReportId { get; set; }
        public int CourseId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
    }
}
