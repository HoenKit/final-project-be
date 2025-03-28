using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
    public class UserManagerDAO : GenericDAO<User>
    {
        public UserManagerDAO(ApplicationDbContext context) : base(context)
        {
        }
    }
}
