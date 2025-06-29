using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Transaction;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface ITransactionRepository : IRepository<Transaction>
    {
        public Task<Transaction> CreateTransaction(TransactionDto dto);
        public PageResult<GetTransactionDto> GetAllTransaction(int page, int pageSize, Guid? UserId, string? sortOption, List<StatusTransactionEnum>? statuses);
    }
}
