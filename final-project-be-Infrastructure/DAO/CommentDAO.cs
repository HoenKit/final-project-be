using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using Microsoft.EntityFrameworkCore;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Infrastructure.DAO
{
    public class CommentDAO : GenericDAO<Comment>, ICommentDAO
    {
        public CommentDAO(ApplicationDbContext context) : base(context)
        {
        }

    }

}
