using final_project_be.Data.Models;
using final_project_be.Dtos.Category;
using final_project_be.Dtos;

namespace final_project_be.Interface
{
    public interface ISubCategoryRepository : IRepository<SubCategory>
    {
        public Task<SubCategory> CreateSubCategory(SubCategoryDto dto);
        public bool DeleteSubCategory(int id);
        public Task<SubCategory> GetSubCategory(int id);
        public Task<SubCategory> UpdateSubCategory(SubCategoryDto dto);
        public PageResult<SubCategory> GetAllSubCategories(int page, int pageSize);
    }
}
