using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Transaction;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{
    public class TransactionRepository : Repository<Transaction>, ITransactionRepository
    {
        private readonly TransactionDAO _transactionDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<AnswerRepository> _logger;
        public TransactionRepository(TransactionDAO transactionDAO, IMapper mapper, ILogger<AnswerRepository> logger) : base(transactionDAO)
        {
            _transactionDAO = transactionDAO;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Transaction> CreateTransaction(TransactionDto dto)
        {
            try
            {
                await _transactionDAO.BeginTransactionAsync();
                var transaction = _mapper.Map<Transaction>(dto);
                await _transactionDAO.AddAsync(transaction);
                await _transactionDAO.CommitTransactionAsync();
                _logger.LogInformation("AddAsync transaction success");
                return transaction;
            }
            catch (Exception ex)
            {
                await _transactionDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding transaction");
                return null;
            }
        }
    }
}
