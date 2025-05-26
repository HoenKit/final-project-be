using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class MentorCertificateDAO : GenericDAO<MentorCertificate>
	{
		public MentorCertificateDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
