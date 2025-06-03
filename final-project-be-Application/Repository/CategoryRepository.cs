using AutoMapper;
using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Category;
using final_project_be_Domain.DTOs;
using final_project_be_Application.Repository;
using Microsoft.Extensions.Logging;
using final_project_be_Application.Interface;

namespace final_project_be_Application.Repository
{
	public class CategoryRepository : Repository<Category>, ICategoryRepository
	{
		private readonly CategoryDAO _categoryDAO;
		private readonly IMapper _mapper;
		private readonly ILogger<CategoryRepository> _logger;

		public CategoryRepository(CategoryDAO categoryDAO, IMapper mapper, ILogger<CategoryRepository> logger) : base(categoryDAO)
		{
			_categoryDAO = categoryDAO;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<Category> CreateCategory(CategoryDto dto)
		{
			try
			{
				await _categoryDAO.BeginTransactionAsync();
				var category = _mapper.Map<Category>(dto);
				await _categoryDAO.AddAsync(category);
				await _categoryDAO.CommitTransactionAsync();

				_logger.LogInformation("AddAsync category success");
				return category;
			}
			catch (Exception ex)
			{
				await _categoryDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when adding category");
				return null;
			}
		}

		public async Task<bool> DeleteCategory(int id)
		{
			try
			{
				await _categoryDAO.BeginTransactionAsync();
				await _categoryDAO.DeleteAsync(id);
				await _categoryDAO.CommitTransactionAsync();

				_logger.LogInformation("DeleteAsync category success");
				return true;
			}
			catch (Exception ex)
			{
				await _categoryDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when delete category");
				return false;
			}
		}

		public PageResult<Category> GetAllCategory(int page, int pageSize)
		{
			try
			{
				var totalCount = _categoryDAO.GetAll().Count();
				var categories = _categoryDAO.GetAll()
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToList();

				_logger.LogInformation("Get category success");

				return new PageResult<Category>(categories, totalCount, page, pageSize);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when getting categories");
				return new PageResult<Category>(new List<Category>(), 0, page, pageSize);
			}

		}

		public async Task<Category> GetCategory(int id)
		{
			try
			{
				await _categoryDAO.BeginTransactionAsync();
				var category = await _categoryDAO.GetByIdAsync(id);
				await _categoryDAO.CommitTransactionAsync();

				_logger.LogInformation("Get category success");
				return category;
			}
			catch (Exception ex)
			{
				await _categoryDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when get category");
				return null;
			}
		}

		public async Task<Category> UpdateCategory(CategoryDto dto)
		{
			try
			{
				await _categoryDAO.BeginTransactionAsync();
				var category = _mapper.Map<Category>(dto);
				await _categoryDAO.UpdateAsync(category);
				await _categoryDAO.CommitTransactionAsync();

				_logger.LogInformation("UpdateAsync category success");
				return category;
			}
			catch (Exception ex)
			{
				await _categoryDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when UpdateAsync category");
				return null;
			}
		}
	}
}
