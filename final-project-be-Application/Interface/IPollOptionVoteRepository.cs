using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.PollOption;
using final_project_be_Domain.DTOs;

namespace final_project_be_Application.Interface
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
