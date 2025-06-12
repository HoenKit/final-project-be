using final_project_be_Domain.DTOs.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using final_project_be_Application.Interface;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        // GET: api/<CategoryController>
        [HttpGet]
        
        public IActionResult GetAll(int? page, int? pageSize)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            int currentSize = pageSize ?? 5;
            var pagedComments = _categoryRepository.GetAllCategory(currentPage, currentSize);
            return Ok(pagedComments);
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _categoryRepository.GetCategory(id));
        }

        // POST api/<CategoryController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _categoryRepository.CreateCategory(categoryDto);
            return Ok(categoryDto);
        }

        // PUT api/<CategoryController>/5
        [HttpPut]
        public async Task<IActionResult> Put(CategoryDto categoryDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            await _categoryRepository.UpdateCategory(categoryDto);
            return Ok(categoryDto);
        }

        // DeleteAsync api/<CategoryController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _categoryRepository.DeleteCategory(id);
            return Ok();
        }
    }
}
