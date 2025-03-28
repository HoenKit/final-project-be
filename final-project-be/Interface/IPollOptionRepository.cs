using final_project_be.Data.Models;
using final_project_be.Dtos.PollOption;
using final_project_be.Dtos;

namespace final_project_be.Interface
{
    public interface IPollOptionRepository : IRepository<PollOption>
    {
        public Task<PollOption> CreatePollOption(PollOptionDto dto);
        public bool DeletePollOption(int id);
        public Task<PollOption> GetPollOption(int id);
        public Task<PollOption> UpdatePollOption(PollOptionDto dto);
        public PageResult<PollOption> GetAllPollOptions(int page, int pageSize);
    }
}
