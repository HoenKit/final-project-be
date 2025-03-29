using final_project_be.Data.Models;
using final_project_be.Data;

namespace final_project_be.DAO
{
    public class SubCategoryDAO : GenericDAO<SubCategory>
    {
        public SubCategoryDAO(ApplicationDbContext context) : base(context)
        {
        }
    }
}
