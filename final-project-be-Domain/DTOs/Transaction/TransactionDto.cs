using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Transaction
{
    public class TransactionDto
    {
        public Guid UserId { get; set; }
        public decimal? Points { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public decimal? Amount { get; set; }
        public string? OrderCode { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
