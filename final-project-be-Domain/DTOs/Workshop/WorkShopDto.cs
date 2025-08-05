using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Workshop
{
    public class WorkShopDto
    {
        public int WorkShopId { get; set; }
        public int MentorId { get; set; }
        public string? Decription { get; set; }
        public string? StreamingLink { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime UpdateAt { get; set; } = DateTime.Now;
    }

    public class WorkShopCreateDto
    {
        public int MentorId { get; set; }
        public string Decription { get; set; }
        public string StreamingLink { get; set; }
    }
}
