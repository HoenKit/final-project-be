using final_project_be.Data.Models;
using final_project_be.Data;

namespace final_project_be.DAO
{
    public class CategoryDAO : GenericDAO<Category>
    {
        public CategoryDAO(ApplicationDbContext context) : base(context)
        {
        }
    }
}
