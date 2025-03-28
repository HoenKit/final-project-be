using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.Category;
using final_project_be.Dtos.Comment;
using final_project_be.Dtos.PollOption;
using final_project_be.Interface;

namespace final_project_be.Repository
{
    public class PollOptionRepository : Repository<PollOption>, IPollOptionRepository
    {
        private readonly PollOptionDAO _polloptionDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<PollOptionRepository> _logger;
        public PollOptionRepository(PollOptionDAO polloptionDAO, IMapper mapper, ILogger<PollOptionRepository> logger) : base(polloptionDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _polloptionDAO = polloptionDAO;
        }

        public async Task<PollOption> CreatePollOption(PollOptionDto dto)
        {
            try
            {
                _polloptionDAO.BeginTransaction();
                var polloption = _mapper.Map<PollOption>(dto);
                _polloptionDAO.Add(polloption);
                _polloptionDAO.CommitTransaction();

                _logger.LogInformation("Add Poll Option success");
                return polloption;
            }
            catch (Exception ex)
            {
                _polloptionDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when adding Poll Option");
                return null;
            }
        }

        public bool DeletePollOption(int id)
        {
            try
            {
                _polloptionDAO.BeginTransaction();
                _polloptionDAO.Delete(id);
                _polloptionDAO.CommitTransaction();

                _logger.LogInformation("Delete Poll Option success");
                return true;
            }
            catch (Exception ex)
            {
                _polloptionDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when delete Poll Option");
                return false;
            }
        }

        public PageResult<PollOption> GetAllPollOptions(int page, int pageSize)
        {
            try
            {
                var totalCount = _polloptionDAO.GetAll().Count();
                var polloptions = _polloptionDAO.GetAll()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get Poll Options success");

                return new PageResult<PollOption>(polloptions, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting Poll Options");
                return new PageResult<PollOption>(new List<PollOption>(), 0, page, pageSize);
            }
        }

        public async Task<PollOption> GetPollOption(int id)
        {

            try
            {
                _polloptionDAO.BeginTransaction();
                var polloption = _polloptionDAO.GetById(id);
                _polloptionDAO.CommitTransaction();

                _logger.LogInformation("Get Poll Options success");
                return polloption;
            }
            catch (Exception ex)
            {
                _polloptionDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get Poll Options");
                return null;
            }
        }

        public async Task<PollOption> UpdatePollOption(PollOptionDto dto)
        {
            try
            {
                _polloptionDAO.BeginTransaction();
                var pollOptions = _mapper.Map<PollOption>(dto);
                _polloptionDAO.Update(pollOptions);
                _polloptionDAO.CommitTransaction();

                _logger.LogInformation("Update Poll Options success");
                return pollOptions;
            }
            catch (Exception ex)
            {
                _polloptionDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update Poll Options");
                return null;
            }
        }
    }
}
