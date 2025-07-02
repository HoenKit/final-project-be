using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.Models
{
    public class UserEmbedding
    {
        [Key]
        public Guid UserId { get; set; }

        public string EmbeddingJson { get; set; }

        public DateTime UpdatedAt { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }

}
