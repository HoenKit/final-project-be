using final_project_be_Domain.DTOs.Users;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Transaction
{
    public class GetTransactionDto
    {
        public int TransactionId { get; set; }
        public Guid UserId { get; set; }
        public decimal? Points { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Status { get; set; }
        public decimal? Amount { get; set; }
        public string? OrderCode { get; set; }
        public DateTime CreateAt { get; set; }
        public UserDto? User { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum StatusTransactionEnum
    {
        Completed,
        Cancel
    }
}
