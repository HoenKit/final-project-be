using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace final_project_be_Domain.Models
{
    public class ReportCourse
    {
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        [ForeignKey("Report")]
        public int ReportId { get; set; }
        [JsonIgnore]
        public Courses? Course { get; set; }
        [JsonIgnore]
        public Report? Report { get; set; }
    }
}
