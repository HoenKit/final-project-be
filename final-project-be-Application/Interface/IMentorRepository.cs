using final_project_be_Domain.DTOs.Notification;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using final_project_be_Domain.DTOs.Mentor;

namespace final_project_be_Application.Interface
{
    public interface IMentorRepository : IRepository<Mentor>
    {   
        public Task<Mentor> CreateMentor(CreateMentorDto dto);
        public Task<bool> DeleteMentor(int id);
        public Task<GetMentorDto> GetMentorandCertificate(int id);
        public Task<Mentor> UpdateMentor(CreateMentorDto dto);
        public PageResult<GetMentorDto> GetAllMentors(int page, int pageSize);
        public Task<GetMentorDto> GetMentorByUserId(Guid userId);
        public Task<MentorbyCourseDto?> GetMentorByCourseIdAsync(int courseId);
        public Task<bool> UpdateInfoBankAsync(Guid userId, InfoBank dto);

    }
}
