using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class PaymentCourseDAO : GenericDAO<PaymentCourse>
	{
		public PaymentCourseDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
