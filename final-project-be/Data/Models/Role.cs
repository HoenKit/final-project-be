using System.ComponentModel.DataAnnotations;

namespace final_project_be.Data.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; } 
        public string RoleName { get; set; } 
        public DateTime CreateAt { get; set; } = DateTime.Now;
        public DateTime? UpdateAt { get; set; } = DateTime.Now;
    }
}
