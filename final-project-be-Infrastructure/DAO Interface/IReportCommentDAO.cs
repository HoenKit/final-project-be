using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IReportCommentDAO : IGenericDAO<ReportComment>
    {
        List<ReportComment> GetByCommentId(int commentId);
        ReportComment GetByReportId(int id);
        void DeleteByReportAndCommentId(int reportId, int commentId);
    }

}
