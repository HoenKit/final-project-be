using final_project_be_Domain.DTOs.Lesson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Module
{
    public class AIGeneratedModule
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<AIGeneratedLesson> Lessons { get; set; }
    }
}
