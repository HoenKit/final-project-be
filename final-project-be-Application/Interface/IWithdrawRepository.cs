using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Withdraw;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface IWithdrawRepository : IRepository<Withdraw>
    {
        public Task<Withdraw> CreateWithdraw (WithdrawDto dto);
        public PageResult<Withdraw> GetAllWithdraw (int page, int pageSize, int? mentorId, string? sortOption, List<WithdrawEnum>? status);
        public Task<Withdraw> UpdateStatus (int withdrawId, string status);
    }
}
