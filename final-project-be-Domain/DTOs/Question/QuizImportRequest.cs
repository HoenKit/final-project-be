using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Question
{
    public class QuizImportRequest
    {
        public IFormFile PdfFile { get; set; }  
        public int LessonId { get; set; }        
        public int Number { get; set; }          
        public string Difficulty { get; set; }   
    }

}
