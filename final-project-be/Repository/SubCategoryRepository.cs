using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos.Category;
using final_project_be.Dtos;
using final_project_be.Interface;

namespace final_project_be.Repository
{
    public class SubCategoryRepository : Repository<SubCategory>, ISubCategoryRepository
    {
        private readonly SubCategoryDAO _subcategoryDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<SubCategoryRepository> _logger;

        public SubCategoryRepository(SubCategoryDAO subcategoryDAO, IMapper mapper, ILogger<SubCategoryRepository> logger) : base(subcategoryDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _subcategoryDAO = subcategoryDAO;
        }


        public async Task<SubCategory> CreateSubCategory(SubCategoryDto dto)
        {
            try
            {
                _subcategoryDAO.BeginTransaction();
                var subcategory = _mapper.Map<SubCategory>(dto);
                _subcategoryDAO.Add(subcategory);
                _subcategoryDAO.CommitTransaction();

                _logger.LogInformation("Add subcategory success");
                return subcategory;
            }
            catch (Exception ex)
            {
                _subcategoryDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when adding subcategory");
                return null;
            }
        }

        public bool DeleteSubCategory(int id)
        {
            try
            {
                _subcategoryDAO.BeginTransaction();
                _subcategoryDAO.Delete(id);
                _subcategoryDAO.CommitTransaction();

                _logger.LogInformation("Delete subcategory success");
                return true;
            }
            catch (Exception ex)
            {
                _subcategoryDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when delete subcategory");
                return false;
            }
        }

        public PageResult<SubCategory> GetAllSubCategories(int page, int pageSize)
        {
            try
            {
                var totalCount = _subcategoryDAO.GetAll().Count();
                var subcategories = _subcategoryDAO.GetAll()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get subcategory success");

                return new PageResult<SubCategory>(subcategories, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting subcategoryies");
                return new PageResult<SubCategory>(new List<SubCategory>(), 0, page, pageSize);
            }
        }

        public async Task<SubCategory> GetSubCategory(int id)
        {
            try
            {
                _subcategoryDAO.BeginTransaction();
                var comment = _subcategoryDAO.GetById(id);
                _subcategoryDAO.CommitTransaction();

                _logger.LogInformation("Get subcategories success");
                return comment;
            }
            catch (Exception ex)
            {
                _subcategoryDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get subcategories");
                return null;
            }

        }

        public async Task<SubCategory> UpdateSubCategory(SubCategoryDto dto)
        {
            try
            {
                _subcategoryDAO.BeginTransaction();
                var subcategory = _mapper.Map<SubCategory>(dto);
                _subcategoryDAO.Update(subcategory);
                _subcategoryDAO.CommitTransaction();

                _logger.LogInformation("Update subcategories success");
                return subcategory;
            }
            catch (Exception ex)
            {
                _subcategoryDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update subcategories");
                return null;
            }
        }
    }
}
