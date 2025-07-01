using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.Models
{
    public class CourseEmbedding
    {
        [Key]
        public int CourseId { get; set; }

        public string EmbeddingJson { get; set; }

        [ForeignKey("CourseId")]
        public Courses Course { get; set; }
    }

}
