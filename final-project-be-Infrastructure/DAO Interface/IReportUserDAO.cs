using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IReportUserDAO : IGenericDAO<ReportUser>
    {
        List<ReportUser> GetByUserId(Guid userId);
        ReportUser GetByReportId(int id);
        void DeleteByReportAndUserId(int reportId, Guid userId);
    }

}
