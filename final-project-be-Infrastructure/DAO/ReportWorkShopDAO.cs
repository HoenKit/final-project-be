using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO
{
    public class ReportWorkShopDAO : GenericDAO<ReportWorkShop>, IReportWorkShopDAO
	{
		private readonly ApplicationDbContext _context;

		public ReportWorkShopDAO(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public List<ReportWorkShop> GetByWorkShopId(int workShopId)
		{
			return _context.reportWorkShop
				.Where(rp => rp.WorkshopId == workShopId)
				.ToList();
		}

		public ReportWorkShop GetByReportId(int id)
		{
			return _context.reportWorkShop
				.FirstOrDefault(r => r.ReportId == id);
		}

		public void DeleteByReportAndWorkShopId(int reportId, int workShopId)
		{
			var reportWorkShops = _context.reportWorkShop
				.Where(r => r.ReportId == reportId && r.WorkshopId == workShopId)
				.ToList();

			if (reportWorkShops.Any())
			{
				_context.RemoveRange(reportWorkShops);
				_context.SaveChanges();
			}
		}
	}
}
