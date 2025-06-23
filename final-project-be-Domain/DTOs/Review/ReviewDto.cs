using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Review
{
    public class ReviewDto
    {
        public int CourseId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; } = false;
        public decimal rate { get; set; }
    }
}
