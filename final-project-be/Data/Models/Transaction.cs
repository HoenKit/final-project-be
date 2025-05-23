using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string? PointChange { get; set; }
        public string? TransactionType { get; set; }
        public string? Status { get; set; }
        public string? PointCost { get; set; }
        public string? Description { get; set; }
        public DateTime ExpiredAt { get; set; }
        public DateTime CreateAt {  get; set; }
        public User? Users { get; set; }

    }
}
