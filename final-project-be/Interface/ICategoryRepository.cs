using final_project_be.Data.Models;
using final_project_be.Dtos.Category;
using final_project_be.Dtos;

namespace final_project_be.Interface
{
    public interface ICategoryRepository : IRepository<Category>
    {
        public Task<Category> CreateCategory(CategoryDto dto);
        public bool DeleteCategory(int id);
        public Task<Category> GetCategory(int id);
        public Task<Category> UpdateCategory(CategoryDto dto);
        public PageResult<Category> GetAllCategory(int page, int pageSize);
    }
}
