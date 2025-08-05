using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Schedule
{
    public class ScheduleDto
    {
        public int ScheduleId { get; set; }
        public int MentorId { get; set; }
        public string ScheduleName { get; set; }
        public DateTime MentorDay { get; set; }
        public DateTime CreateAt { get; set; }
        public int CourseId { get; set; }
    }

    public class UserScheduleDto
    {
        public Guid UserId { get; set; }
        public int ScheduleId { get; set; }
    }
}
