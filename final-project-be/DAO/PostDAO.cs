using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
    public class PostDAO : GenericDAO<Post>
    {
        public PostDAO(ApplicationDbContext context) : base(context)
        {
        }
    }
}
