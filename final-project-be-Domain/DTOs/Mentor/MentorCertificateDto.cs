using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Mentor
{
    public class MentorCertificateDto
    {
        public int MentorCertificateId { get; set; }
        public int MentorId { get; set; }
        public IFormFile? FileUrl { get; set; }
        public string CertificateName { get; set; }
    }
    public class GetMentorCertificateDto
    {
        public int MentorCertificateId { get; set; }
        public string? FileUrl { get; set; }
        public string CertificateName { get; set; }
    }


}
