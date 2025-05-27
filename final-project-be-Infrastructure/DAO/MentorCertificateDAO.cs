using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;

namespace final_project_be_Infrastructure.DAO
{
	public class MentorCertificateDAO : GenericDAO<MentorCertificate>
	{
		public MentorCertificateDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
