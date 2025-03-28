using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class UserMetadata
    {
        public int UserMetadataId { get; set; }
        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string? Gender { get; set; }
        public string? Avatar { get; set; }
        public string? Address { get; set; }
        public User? User { get; set; }
    }
}
