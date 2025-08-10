using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
	public interface IReportWorkShopDAO : IGenericDAO<ReportWorkShop>
	{
		List<ReportWorkShop> GetByWorkShopId(int workShopId);
		ReportWorkShop GetByReportId(int id);
		void DeleteByReportAndWorkShopId(int reportId, int workShopId);
	}
}
