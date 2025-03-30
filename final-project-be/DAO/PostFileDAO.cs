using final_project_be.Data;
using final_project_be.Data.Models;

namespace final_project_be.DAO
{
	public class PostFileDAO : GenericDAO<PostFile>
	{
		public PostFileDAO(ApplicationDbContext context) : base(context)
		{
		}
	}
}
