using final_project_be.Data;
using final_project_be.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace final_project_be.DAO
{
    public class PostManagerDAO : GenericDAO<Post>
    {
        private readonly ApplicationDbContext _context;
        public PostManagerDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Post> GetAllPosts()
        {
            return _context.posts
                .Include(p => p.User)
                .Include(p => p.SubCategory)
                .Include(p => p.PostFiles);
        }

        public Post? GetPostById(int postId)
        {
            return _context.posts
                .Include(p => p.User)
                .Include(p => p.SubCategory)
                .Include(p => p.PostFiles)
                .FirstOrDefault(p => p.PostId == postId);
        }

    }
}
