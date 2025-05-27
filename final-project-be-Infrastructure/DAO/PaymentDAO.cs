using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;

namespace final_project_be_Infrastructure.DAO
{
	public class PaymentDAO : GenericDAO<Payment>
	{
		public PaymentDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
