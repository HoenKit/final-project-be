using final_project_be.Data.Models;
using final_project_be.Data;

namespace final_project_be.DAO
{
    public class PollOptionDAO : GenericDAO<PollOption>
    {
        public PollOptionDAO(ApplicationDbContext context) : base(context)
        {
        }
    }
}
