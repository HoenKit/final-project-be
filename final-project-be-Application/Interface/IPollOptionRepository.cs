using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.PollOption;
using final_project_be_Domain.DTOs;

namespace final_project_be_Application.Interface
{
    public interface IPollOptionRepository : IRepository<PollOption>
    {
        public Task<PollOption> CreatePollOption(PollOptionDto dto);
        public Task<bool> DeletePollOption(int id);
        public Task<PollOption> GetPollOption(int id);
        public Task<PollOption> UpdatePollOption(PollOptionDto dto);
        public PageResult<PollOption> GetAllPollOptions(int page, int pageSize);
    }
}
