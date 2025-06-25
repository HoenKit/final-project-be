using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Review
{
    public class ReviewResponseDto
    {
        public int ReviewId { get; set; }
        public int CourseId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; }
        public bool IsDeleted { get; set; } = false;
        public decimal Rate { get; set; }
        public DateTime CreateAt { get; set; } 
        public DateTime UpdateAt { get; set; }
        public User? User { get; set; }
    }
}
