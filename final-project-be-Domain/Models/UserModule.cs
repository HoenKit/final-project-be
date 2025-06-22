using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be_Domain.Models
{
    public class UserModule
    {
        [ForeignKey("Module")]
        public int ModuleId { get; set; }
        [ForeignKey("User")]
        public Guid UserId {  get; set; }
        public string Status { get; set; } = "not started";
        public float? Percentage { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Module? Module { get; set; }
        public User? User { get; set; }
    }
}
