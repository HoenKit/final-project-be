using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Transaction;
using final_project_be_Domain.DTOs.Users;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using Microsoft.EntityFrameworkCore;
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

        public PageResult<GetTransactionDto> GetAllTransaction(int page, int pageSize, Guid? UserId, string? sortOption, List<StatusTransactionEnum>? statuses)
        {
            try
            {
                var query = _transactionDAO.GetAll()
                    .Include(c => c.Users)
                    .ThenInclude(c => c.UserMetaData)
                    .Where(p => statuses == null || statuses.Count == 0 || statuses.Select(s => s.ToString()).Contains(p.Status));


                if (UserId.HasValue && UserId != Guid.Empty)
                    query = query.Where(p => p.UserId == UserId.Value);

                query = sortOption?.ToLower() switch
                {
                    "asc_date" => query.OrderBy(c => c.CreateAt),
                    "desc_date" => query.OrderByDescending(c => c.CreateAt),
                    _ => query.OrderByDescending(c => c.CreateAt)
                };

                var totalCount = query.Count();

                var transaction = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var transactionDto = transaction.Select(p => new GetTransactionDto
                {
                    TransactionId = p.TransactionId,
                    UserId = p.UserId,
                    Points = p.Points,
                    PaymentMethod = p.PaymentMethod,
                    Status = p.Status,
                    Amount = p.Amount,
                    OrderCode = p.OrderCode,
                    CreateAt = p.CreateAt,
                    User = new UserDto
                    {
                        Email = p.Users.Email,
                        Phone = p.Users.Phone
                    }
                }).ToList();

                _logger.LogInformation("Get filtered transaction success");
                return new PageResult<GetTransactionDto>(transactionDto, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting filtered transactions");
                return new PageResult<GetTransactionDto>(new List<GetTransactionDto>(), 0, page, pageSize);
            }
        }
    }
}
