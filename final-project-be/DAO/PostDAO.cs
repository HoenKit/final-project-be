using System.Linq.Expressions;
using final_project_be.Data;
using final_project_be.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace final_project_be.DAO
{
    public class PostDAO : GenericDAO<Post>
    {
        public PostDAO(ApplicationDbContext context) : base(context)
        {
        }
    }
}
