using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.Models
{
    public class Withdraw
    {
        [Key]
        public int WithdrawId { get; set; }
        public int MentorId { get; set; }
        [ForeignKey("MentorId")]
        public Mentor? Mentor { get; set; }
        public decimal? Points { get; set; }
        public decimal? Amount { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public string? Status { get; set; } = "Pending";
    }
}
