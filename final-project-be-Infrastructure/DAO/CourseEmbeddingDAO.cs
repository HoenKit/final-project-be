using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO
{
    public class CourseEmbeddingDAO : GenericDAO<CourseEmbedding>, ICourseEmbeddingDAO
    {
        private readonly ApplicationDbContext _context;

        public CourseEmbeddingDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<List<CourseEmbedding>> GetAllAsync() =>
            _context.CourseEmbeddings
                .Include(e => e.Course)
                    .ThenInclude(c => c.Mentor)
                        .ThenInclude(m => m.User)
                            .ThenInclude(u => u.UserMetaData)
                .Include(e => e.Course)
                    .ThenInclude(c => c.Reviews.Where(r => !r.IsDeleted))
                .ToListAsync();

        public Task<CourseEmbedding?> GetByIdAsync(int id) =>
            _context.CourseEmbeddings
                .Include(e => e.Course)
                .FirstOrDefaultAsync(c => c.CourseId == id);
    }

}
