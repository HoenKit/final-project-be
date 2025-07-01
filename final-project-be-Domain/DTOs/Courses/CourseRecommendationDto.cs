using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Courses
{
    public class CourseRecommendationDto
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string Image { get; set; }
        public double Score { get; set; }
    }

}
