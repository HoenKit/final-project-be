using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Payment;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface IPaymentRepositoty 
    {
        public Task<bool> BuyPremiumAsync(Guid userId, int planId);
        public Task<BuyCourseResult> BuyCourseAsync(Guid userId, int courseId, int couponId);
        public PageResult<GetPaymentDto> GetAll(int page, int pageSize, Guid? UserId, string? sortOption, List<ServiceTypeEnum>? ServiceType);
        public Task<List<MothlyStatPaymentDto>> GetStatisticsByMonth(int? year);
        public Task<IEnumerable<MembershipPlan>> GetAllMembershipplanAsync();
    }
}
