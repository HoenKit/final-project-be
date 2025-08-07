using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace final_project_be_Domain.Models
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
        [JsonIgnore]
        public Post? Post { get; set; }
    }
}
