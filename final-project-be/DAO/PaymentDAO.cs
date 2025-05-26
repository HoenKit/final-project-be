using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class PaymentDAO : GenericDAO<Payment>
	{
		public PaymentDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
