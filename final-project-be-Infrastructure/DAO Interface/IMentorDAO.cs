using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IMentorDAO : IGenericDAO<Mentor>
    {
        Task<Mentor> GetMentorandcertificate(int id);
        Task<Mentor> GetMentorByUserId(Guid userId);
        Task<Mentor> GetMentorinCourseAsync(int mentorId);
        Task<MentorbyCourseDto?> GetMentorByCourseIdAsync(int courseId);
    }

}
