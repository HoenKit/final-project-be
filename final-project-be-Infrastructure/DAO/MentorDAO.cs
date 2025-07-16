using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class MentorDAO : GenericDAO<Mentor>, IMentorDAO
    {
        private readonly ApplicationDbContext _context;

        public MentorDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Mentor> GetMentorandcertificate(int id)
            => await _context.Mentors
                .Include(m => m.MentorCertificates)
                .FirstOrDefaultAsync(m => m.MentorId == id);

        public async Task<Mentor> GetMentorByUserId(Guid userId)
            => await _context.Mentors.FirstOrDefaultAsync(m => m.UserId == userId);

        public async Task<Mentor> GetMentorinCourseAsync(int mentorId)
            => await _context.Mentors.Include(u => u.User).FirstOrDefaultAsync(m => m.MentorId == mentorId);

        public async Task<MentorbyCourseDto?> GetMentorByCourseIdAsync(int courseId)
        {
            var mentor = await _context.Courses
                .Where(c => c.CourseId == courseId)
                .Select(c => new MentorbyCourseDto
                {
                    MentorId = c.Mentor.MentorId,
                    UserId = c.Mentor.UserId,
                    JobTitle = c.Mentor.JobTitle,
                    StudyLevel = c.Mentor.StudyLevel,
                    Introduction = c.Mentor.Introduction,
                    Signature = c.Mentor.Signature,
                    FirstName = c.Mentor.User.UserMetaData.FirstName,
                    LastName = c.Mentor.User.UserMetaData.LastName,
                })
                .FirstOrDefaultAsync();

            return mentor;
        }
    }

}
