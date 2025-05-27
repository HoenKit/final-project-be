using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;

namespace final_project_be_Infrastructure.DAO
{
	public class PaymentCourseDAO : GenericDAO<PaymentCourse>
	{
		public PaymentCourseDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
