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
        public Task<bool> BuyCourseAsync(Guid userId, int courseId, int CouponId);
    }
}
