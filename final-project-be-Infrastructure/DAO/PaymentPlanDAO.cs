using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace final_project_be_Infrastructure.DAO
{
    public class PaymentPlanDAO : GenericDAO<PaymentPlan>, IPaymentPlanDAO
    {
        public PaymentPlanDAO(ApplicationDbContext context) : base(context)
        {
        }

    }

}
