using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Withdraw;
using final_project_be_Domain.DTOs.Workshop;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface IWorkshopRepository : IRepository<WorkShop>
    {
        public Task<WorkShop?> CreateWorkshopAsync(WorkShopCreateDto dto);
        public PageResult<WorkShop> GetAllWorkshop(int page, int pageSize);
        public Task<WorkShop> UpdateWorkshop(WorkShopDto dto);
        public Task<WorkShop> GetWorkshop(int id);
        public Task<bool> DeleteCategory(int id);
    }
}
