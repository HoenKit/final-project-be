using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Withdraw
{
    public class UpdateWithdrawDto
    {
        public int WithdrawId { get; set; }
        public int MentorId { get; set; }
        public decimal? Points { get; set; }
        public decimal? Amount { get; set; }
        public string status { get; set; }
        public UpdateMentorDto Mentor { get; set; }
    }

    public class UpdateMentorDto
    {
        public int MentorId { get; set; }
        public Guid UserId { get; set; }

        public UpdateUserDto User { get; set; }
    }

    public class UpdateUserDto
    {
        public Guid UserId { get; set; }
        public decimal? Point { get; set; }
    }
}
