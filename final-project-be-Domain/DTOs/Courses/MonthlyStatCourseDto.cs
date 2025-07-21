using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Courses
{
    public class MonthlyStatCourseDto
    {
        public string Time { get; set; }
        public int TotalCoursesCreated { get; set; }
        public int TotalStudentsEnrolled { get; set; }
        public decimal TotalEarnings { get; set; }
    }
}