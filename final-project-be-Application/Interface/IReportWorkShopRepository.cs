using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
	public interface IReportWorkShopRepository : IRepository<ReportWorkShop>
	{
        public Task<ReportWorkShop> CreateReportWorkShop(ReportWorkShopDto dto);
        public PageResult<ReportWorkShop> GetAllReportWorkShops(int page, int pageSize);
        public Task<ReportWorkShop> GetReportWorkShop(int id);
        public PageResult<GroupedReportDto<int, ReportWorkShop>> GetGroupedReportWorkShops(int page, int pageSize);
        public Task<bool> DeleteReportsByWorkShopId(int workShopId);

    }
}
