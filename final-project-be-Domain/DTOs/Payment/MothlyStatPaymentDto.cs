using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Payment
{
    public class MothlyStatPaymentDto
    {
        public string Time {  get; set; }
        public decimal TotalPoint { get; set; }
        public int TotalPremium { get; set; }
    }
}
