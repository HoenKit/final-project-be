using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Withdraw;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{
    public class WithdrawRepository : Repository<Withdraw>, IWithdrawRepository
    {
        private readonly IWithdrawDAO _withdrawDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<WithdrawRepository> _logger;
        public WithdrawRepository(IWithdrawDAO withdrawDAO, IMapper mapper, ILogger<WithdrawRepository> logger) : base(withdrawDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _withdrawDAO = withdrawDAO;
        }

        public async Task<Withdraw> CreateWithdraw(WithdrawDto dto)
        {
            try
            {
                await _withdrawDAO.BeginTransactionAsync();
                var withdraw = _mapper.Map<Withdraw>(dto);
                await _withdrawDAO.AddAsync(withdraw);
                await _withdrawDAO.CommitTransactionAsync();
                _logger.LogInformation("Add withdraw success");
                return withdraw;
            }
            catch (Exception ex)
            {
                await _withdrawDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding withdraw");
                return null;
            }
        }

        public PageResult<Withdraw> GetAllWithdraw(int page, int pageSize, int? mentorId, string? sortOption, List<WithdrawEnum>? status, bool isCurrentMonth = false)
        {
            var query = _withdrawDAO.GetAll();

            if (mentorId.HasValue)
            {
                query = query.Where(w => w.MentorId == mentorId.Value);
            }

            if (status != null && status.Any())
            {
                var statusStrings = status.Select(s => s.ToString()).ToList();
                query = query.Where(w => statusStrings.Contains(w.Status));
            }

            if (isCurrentMonth)
            {
                var now = DateTime.UtcNow;
                query = query.Where(w => w.CreateAt.Month == now.Month && w.CreateAt.Year == now.Year);
            }

            query = sortOption?.ToLower() switch
            {
                "asc_date" => query.OrderBy(c => c.CreateAt),
                "desc_date" => query.OrderByDescending(c => c.CreateAt),
                _ => query.OrderByDescending(c => c.CreateAt)
            };

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(w => w.CreateAt) 
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PageResult<Withdraw>(items, totalCount, page, pageSize);
        }


        public async Task<Withdraw> UpdateStatus(int withdrawId, string status)
        {
            try
            {
                await _withdrawDAO.BeginTransactionAsync();

                var withdraw = await _withdrawDAO.GetByIdAsync(withdrawId);
                if (withdraw == null)
                {
                    _logger.LogWarning($"Withdraw with ID {withdrawId} not found.");
                    await _withdrawDAO.RollbackTransactionAsync();
                    return null;
                }

                withdraw.Status = status;
                await _withdrawDAO.UpdateAsync(withdraw);

                await _withdrawDAO.CommitTransactionAsync();

                _logger.LogInformation("Update withdraw success");
                return withdraw;
            }
            catch (Exception ex)
            {
                await _withdrawDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when updating withdraw");
                return null;
            }
        }
    }
}
