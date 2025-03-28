using final_project_be.Data;
using final_project_be.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace final_project_be.DAO
{
    public class CommentDAO : GenericDAO<Comment>
    {
        public CommentDAO(ApplicationDbContext context) : base(context)
        {
        }
    }
}
