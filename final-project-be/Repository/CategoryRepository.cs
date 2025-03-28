using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.Category;
using final_project_be.Dtos.Comment;
using final_project_be.Interface;

namespace final_project_be.Repository
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
                _categoryDAO.BeginTransaction();
                var category = _mapper.Map<Category>(dto);
                _categoryDAO.Add(category);
                _categoryDAO.CommitTransaction();

                _logger.LogInformation("Add category success");
                return category;
            }
            catch (Exception ex)
            {
                _categoryDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when adding category");
                return null;
            }
        }

        public bool DeleteCategory(int id)
        {
            try
            {
                _categoryDAO.BeginTransaction();
                _categoryDAO.Delete(id);
                _categoryDAO.CommitTransaction();

                _logger.LogInformation("Delete category success");
                return true;
            }
            catch (Exception ex)
            {
                _categoryDAO.RollbackTransaction();
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
                _categoryDAO.BeginTransaction();
                var category = _categoryDAO.GetById(id);
                _categoryDAO.CommitTransaction();

                _logger.LogInformation("Get category success");
                return category;
            }
            catch (Exception ex)
            {
                _categoryDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get category");
                return null;
            }
        }

        public async Task<Category> UpdateCategory(CategoryDto dto)
        {
            try
            {
                _categoryDAO.BeginTransaction();
                var category = _mapper.Map<Category>(dto);
                _categoryDAO.Update(category);
                _categoryDAO.CommitTransaction();

                _logger.LogInformation("Update category success");
                return category;
            }
            catch (Exception ex)
            {
                _categoryDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update category");
                return null;
            }
        }
    }
}
