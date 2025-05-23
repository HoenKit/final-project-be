using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class Certificate
    {
        [Key]
        public int CertificateId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string CertificateName { get; set; }
        public string IssuedBy { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime ExpiredDate { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public Courses? Course { get; set; }
        public User? User { get; set; }
        
    }
}
