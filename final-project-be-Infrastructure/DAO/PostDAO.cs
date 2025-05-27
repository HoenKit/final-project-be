using System.Linq.Expressions;
using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class PostDAO : GenericDAO<Post>
    {
        private readonly ApplicationDbContext _context;
        public PostDAO(ApplicationDbContext context) : base(context)
        {
           _context = context;
        }

        //Add to Delete Post
        public Post? GetPostWithFilesAndComments(int id)
        {
            return GetAll()
                .Include(p => EF.Property<ICollection<PostFile>>(p, "PostFiles"))
                .Include(p => EF.Property<ICollection<Comment>>(p, "Comments"))
                .FirstOrDefault(p => EF.Property<int>(p, "PostId") == id);
        }

        public Post GetPostandUser(int postId)
        {
            return _context.posts
                .Include(p => p.User)
                .ThenInclude(u => u.UserMetaData)
                .FirstOrDefault(p => p.PostId == postId);
        }
    }
}
