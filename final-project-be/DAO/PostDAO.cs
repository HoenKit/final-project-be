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
        //Update SearchPosts
        public IEnumerable<Post> SearchPosts(string query)
        {
            Expression<Func<Post, bool>> predicate = p => p.Title.Contains(query) || p.Content.Contains(query);

            return Find(predicate);
        }
        //Get Get Post By UserId
        public IEnumerable<Post> GetByUserId(Guid userId)
        {
            return Find(p => p.UserId == userId);
        }
    }
}
