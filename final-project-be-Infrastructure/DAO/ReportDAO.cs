using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;

namespace final_project_be_Infrastructure.DAO
{
    public class ReportDAO : GenericDAO<Report>
    {
        public ReportDAO(ApplicationDbContext context) : base(context)
        {
        }
    }
}
