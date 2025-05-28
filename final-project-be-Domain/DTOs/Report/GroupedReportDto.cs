using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.Report
{
    public class GroupedReportDto<TId, T>
    {
        public TId Id { get; set; }
        public int ReportCount { get; set; }
        public List<T> Reports { get; set; } = new();
    }
}
