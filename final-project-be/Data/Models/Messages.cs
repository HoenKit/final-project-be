using System.ComponentModel.DataAnnotations;

namespace final_project_be.Data.Models
{
    public class Messages
    {
        [Key]
        public int MessageId {  get; set; }
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public string Content { get; set; }
        public DateTime SentAt { get; set; }
        public Mentor? Mentors { get; set; }
        public User? User { get; set; }
    }
}
