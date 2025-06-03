using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Category;
using final_project_be_Domain.DTOs;

namespace final_project_be_Application.Interface
{
	public interface ICategoryRepository : IRepository<Category>
	{
		public Task<Category> CreateCategory(CategoryDto dto);
		public Task<bool> DeleteCategory(int id);
		public Task<Category> GetCategory(int id);
		public Task<Category> UpdateCategory(CategoryDto dto);
		public PageResult<Category> GetAllCategory(int page, int pageSize);
	}
}
