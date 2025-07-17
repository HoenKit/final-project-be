using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Withdraw
{
    public class WithdrawDto
    {
        public int MentorId { get; set; }
        public decimal? Points { get; set; }
        public decimal? Amount { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum WithdrawEnum
    {
        Pending,
        Accepted,
        Refused
    }
}
