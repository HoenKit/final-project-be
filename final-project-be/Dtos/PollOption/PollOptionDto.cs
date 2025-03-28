using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace final_project_be.Dtos.PollOption
{
    public class PollOptionDto
    {
        public int PollOptionId { get; set; }
        public int PostId { get; set; }
        public string Title { get; set; }
    }
}
