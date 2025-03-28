using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos.PollOption;
using final_project_be.Dtos;
using final_project_be.Interface;

namespace final_project_be.Repository
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
                _PollOptionDAO.BeginTransaction();
                var PollOption = _mapper.Map<PollOption>(dto);
                _PollOptionDAO.Add(PollOption);
                _PollOptionDAO.CommitTransaction();

                _logger.LogInformation("Add PollOption success");
                return PollOption;
            }
            catch (Exception ex)
            {
                _PollOptionDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when adding PollOption");
                return null;
            }
        }

        public bool DeletePollOption(int id)
        {
            try
            {
                _PollOptionDAO.BeginTransaction();
                _PollOptionDAO.Delete(id);
                _PollOptionDAO.CommitTransaction();

                _logger.LogInformation("Delete PollOption success");
                return true;
            }
            catch (Exception ex)
            {
                _PollOptionDAO.RollbackTransaction();
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
                _PollOptionDAO.BeginTransaction();
                var PollOption = _PollOptionDAO.GetById(id);
                _PollOptionDAO.CommitTransaction();

                _logger.LogInformation("Get PollOption success");
                return PollOption;
            }
            catch (Exception ex)
            {
                _PollOptionDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get PollOption");
                return null;
            }

        }

        public async Task<PollOption> UpdatePollOption(PollOptionDto dto)
        {
            try
            {
                _PollOptionDAO.BeginTransaction();
                var PollOption = _mapper.Map<PollOption>(dto);
                _PollOptionDAO.Update(PollOption);
                _PollOptionDAO.CommitTransaction();

                _logger.LogInformation("Update PollOption success");
                return PollOption;
            }
            catch (Exception ex)
            {
                _PollOptionDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update PollOption");
                return null;
            }
        }
    }
}
