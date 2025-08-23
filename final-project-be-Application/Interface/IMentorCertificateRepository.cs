using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Module;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface IMentorCertificateRepository : IRepository<MentorCertificate>
    {
        public Task<MentorCertificate> CreateMentorCertificate(MentorCertificateDto dto);
        public Task<bool> DeleteMentorCertificate(int id);
        public Task<GetMentorCertificateDto> GetMentorCertificate(int id);
        public Task<MentorCertificate> UpdateMentorCertificate(MentorCertificateDto dto);
        public Task<ICollection<MentorCertificate>> GetMentorCertificatesByUserId(Guid userId);
        public PageResult<GetMentorCertificateDto> GetAllMentorCertificates(int page, int pageSize);

        public Task<ICollection<GetMentorCertificateDto>> GetAllMentorCertificatesByMentorId(int MentorId);
    }
}
