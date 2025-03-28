using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace final_project_be.Data.Models
{
    public class PostFile
    {
        [Key]
        public int PostFileId { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }
        public string FileUrl { get; set; }
        public string PostFileType { get; set; }
        public bool? IsDeleted { get; set; } = false;

        public Post? Post { get; set; }
    }
}
