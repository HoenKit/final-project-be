using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.SearchResult
{
    public class SearchResultDto
    {
        public List<object> Users { get; set; } = new();
        public List<object> Posts { get; set; } = new();
        public List<object> Courses { get; set; } = new();
        public int TotalResults { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }

}
