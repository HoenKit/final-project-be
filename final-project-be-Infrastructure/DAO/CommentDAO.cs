using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class CommentDAO : GenericDAO<Comment>
    {
        public CommentDAO(ApplicationDbContext context) : base(context)
        {
        }
    }
}
