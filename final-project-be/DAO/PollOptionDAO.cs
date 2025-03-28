using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
    public class PollOptionDAO : GenericDAO<PollOption>
    {
        public PollOptionDAO(ApplicationDbContext context) : base(context)
        {
        }
    }
}
