using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
	public class MentorDAO : GenericDAO<Mentor>
	{

        private readonly ApplicationDbContext _context;
        public MentorDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<Mentor> GetMentorandcertificate(int id) => await _context.Mentors.Include(m => m.MentorCertificates).FirstOrDefaultAsync(m => m.MentorId == id);
        public async Task<Mentor> GetMentorByUserId(Guid userId) => await _context.Mentors.FirstOrDefaultAsync(m => m.UserId == userId);
        public async Task<Mentor> GetMentorinCourseAsync(int MentorId) => await _context.Mentors.Include(u => u.User).FirstOrDefaultAsync(m => m.MentorId == MentorId);
    }
}
