using AutoMapper;
using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.PollOption;
using final_project_be_Domain.DTOs;
using final_project_be_Application.Interface;
using Microsoft.Extensions.Logging;

namespace final_project_be_Application.Repository
{
    public class PollOptionRepository : Repository<PollOption>, IPollOptionRepository
    {
        private readonly PollOptionDAO _PollOptionDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<PollOptionRepository> _logger;

        public PollOptionRepository(PollOptionDAO PollOptionDAO, IMapper mapper, ILogger<PollOptionRepository> logger) : base(PollOptionDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _PollOptionDAO = PollOptionDAO;
        }

        public async Task<PollOption> CreatePollOption(PollOptionDto dto)
        {
            try
            {
                await _PollOptionDAO.BeginTransactionAsync();
                var PollOption = _mapper.Map<PollOption>(dto);
                await _PollOptionDAO.AddAsync(PollOption);
                await _PollOptionDAO.CommitTransactionAsync();

                _logger.LogInformation("AddAsync PollOption success");
                return PollOption;
            }
            catch (Exception ex)
            {
                await _PollOptionDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding PollOption");
                return null;
            }
        }

        public async Task<bool> DeletePollOption(int id)
        {
            try
            {
                await _PollOptionDAO.BeginTransactionAsync();
                await _PollOptionDAO.DeleteAsync(id);
                await _PollOptionDAO.CommitTransactionAsync();

                _logger.LogInformation("DeleteAsync PollOption success");
                return true;
            }
            catch (Exception ex)
            {
                await _PollOptionDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when delete PollOption");
                return false;
            }
        }

        public PageResult<PollOption> GetAllPollOptions(int page, int pageSize)
        {
            try
            {
                var totalCount = _PollOptionDAO.GetAll().Count();
                var PollOptions = _PollOptionDAO.GetAll()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get PollOptions success");

                return new PageResult<PollOption>(PollOptions, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting PollOptions");
                return new PageResult<PollOption>(new List<PollOption>(), 0, page, pageSize);
            }
        }

        public async Task<PollOption> GetPollOption(int id)
        {
            try
            {
                await _PollOptionDAO.BeginTransactionAsync();
                var PollOption = await _PollOptionDAO.GetByIdAsync(id);
                await _PollOptionDAO.CommitTransactionAsync();

                _logger.LogInformation("Get PollOption success");
                return PollOption;
            }
            catch (Exception ex)
            {
                await _PollOptionDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when get PollOption");
                return null;
            }

        }

        public async Task<PollOption> UpdatePollOption(PollOptionDto dto)
        {
            try
            {
                await _PollOptionDAO.BeginTransactionAsync();
                var PollOption = _mapper.Map<PollOption>(dto);
                await _PollOptionDAO.UpdateAsync(PollOption);
                await _PollOptionDAO.CommitTransactionAsync();

                _logger.LogInformation("UpdateAsync PollOption success");
                return PollOption;
            }
            catch (Exception ex)
            {
                await _PollOptionDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when UpdateAsync PollOption");
                return null;
            }
        }
    }
}
