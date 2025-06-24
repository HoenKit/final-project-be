using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public decimal? Points { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public decimal? Amount { get; set; }
        public string? OrderCode { get; set; }
        public DateTime CreateAt {  get; set; }
        public User? Users { get; set; }

    }
}
