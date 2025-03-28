using final_project_be.Data.Models;
using final_project_be.Dtos.PollOption;
using final_project_be.Dtos;

namespace final_project_be.Interface
{
    public interface IPollOptionVoteRepository : IRepository<PollOptionVote>
    {
        public Task<PollOptionVote> CreatePollOptionVote(PollOptionVoteDto dto);
        public bool DeletePollOptionVote(int id);
        public Task<PollOptionVote> GetPollOptionVote(int id);
        public Task<PollOptionVote> UpdatePollOptionVote(PollOptionVoteDto dto);
        public PageResult<PollOptionVote> GetAllPollOptionVotes(int page, int pageSize);
    }
}
