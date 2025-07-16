using final_project_be_Domain.DTOs.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Payment
{
    public class GetPaymentDto
    {
        public int PaymentId { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string ServiceType { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ServiceTypeEnum
    {
        Course,
        Membership
    }
}
