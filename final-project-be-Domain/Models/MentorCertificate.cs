using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
    public class MentorCertificate
    {
        [Key]
        public int MentorCertificateId { get; set; }
        [ForeignKey("Mentor")]
        public int MentorId { get; set; }
        public string FileUrl { get; set; }
        public string CertificateName { get; set; }
        public Mentor? Mentor { get; set; }

    }
}
