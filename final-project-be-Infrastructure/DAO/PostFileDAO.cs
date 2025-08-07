using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class PostFileDAO : GenericDAO<PostFile>, IPostFileDAO
    {
        private readonly ApplicationDbContext _context;
        public PostFileDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<PostFile>> GetByPostIdAsync(int postId)
           => await _context.postFiles.Where(m => m.PostId == postId).ToListAsync();
    }

}
