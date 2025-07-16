using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;

namespace final_project_be_Infrastructure.DAO
{
    public class PollOptionVoteDAO : GenericDAO<PollOptionVote>, IPollOptionVoteDAO
    {
        public PollOptionVoteDAO(ApplicationDbContext context) : base(context)
        {
        }
    }

}
