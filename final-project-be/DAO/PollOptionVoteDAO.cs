using final_project_be.Data.Models;
using final_project_be.Data;

namespace final_project_be.DAO
{
    public class PollOptionVoteDAO : GenericDAO<PollOptionVote>
    {
        public PollOptionVoteDAO(ApplicationDbContext context) : base(context)
        {
        }
    }
}
