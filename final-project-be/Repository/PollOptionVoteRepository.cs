using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos.PollOption;
using final_project_be.Dtos;
using final_project_be.Interface;

namespace final_project_be.Repository
{
    public class PollOptionVoteRepository : Repository<PollOptionVote>, IPollOptionVoteRepository
    {
        private readonly PollOptionVoteDAO _pollOptionVoteDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<PollOptionVoteRepository> _logger;
        public PollOptionVoteRepository(PollOptionVoteDAO pollOptionVoteDAO, IMapper mapper, ILogger<PollOptionVoteRepository> logger) : base(pollOptionVoteDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _pollOptionVoteDAO = pollOptionVoteDAO;
        }

        public async Task<PollOptionVote> CreatePollOptionVote(PollOptionVoteDto dto)
        {
            try
            {
                _pollOptionVoteDAO.BeginTransaction();
                var pollOptionVote = _mapper.Map<PollOptionVote>(dto);
                _pollOptionVoteDAO.Add(pollOptionVote);
                _pollOptionVoteDAO.CommitTransaction();

                _logger.LogInformation("Add Poll Option Vote success");
                return pollOptionVote;
            }
            catch (Exception ex)
            {
                _pollOptionVoteDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when adding Poll Option Vote");
                return null;
            }
        }

        public bool DeletePollOptionVote(int id)
        {
            try
            {
                _pollOptionVoteDAO.BeginTransaction();
                _pollOptionVoteDAO.Delete(id);
                _pollOptionVoteDAO.CommitTransaction();

                _logger.LogInformation("Delete Poll Option Vote success");
                return true;
            }
            catch (Exception ex)
            {
                _pollOptionVoteDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when delete Poll Option Vote");
                return false;
            }
        }

        public  PageResult<PollOptionVote> GetAllPollOptionVotes(int page, int pageSize)
        {
            try
            {
                var totalCount = _pollOptionVoteDAO.GetAll().Count();
                var polloptionvotes = _pollOptionVoteDAO.GetAll()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get Poll Option Vote success");

                return new PageResult<PollOptionVote>(polloptionvotes, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting Poll Option Vote");
                return new PageResult<PollOptionVote>(new List<PollOptionVote>(), 0, page, pageSize);
            }
        }

        public async Task<PollOptionVote> GetPollOptionVote(int id)
        {

            try
            {
                _pollOptionVoteDAO.BeginTransaction();
                var polloptionvote = _pollOptionVoteDAO.GetById(id);
                _pollOptionVoteDAO.CommitTransaction();

                _logger.LogInformation("Get Poll Option Votes success");
                return polloptionvote;
            }
            catch (Exception ex)
            {
                _pollOptionVoteDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get Poll Option Vote");
                return null;
            }
        }

        public async Task<PollOptionVote> UpdatePollOptionVote(PollOptionVoteDto dto)
        {
            try
            {
                _pollOptionVoteDAO.BeginTransaction();
                var pollOptionvote = _mapper.Map<PollOptionVote>(dto);
                _pollOptionVoteDAO.Update(pollOptionvote);
                _pollOptionVoteDAO.CommitTransaction();

                _logger.LogInformation("Update Poll Option Votes success");
                return pollOptionvote;
            }
            catch (Exception ex)
            {
                _pollOptionVoteDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update Poll Option Votes");
                return null;
            }
        }
    }
}
