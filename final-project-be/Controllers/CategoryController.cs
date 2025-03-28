using final_project_be.Dtos.Category;
using final_project_be.Dtos.Comment;
using final_project_be.Interface;
using final_project_be.Repository;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(ICategoryRepository categoryRepository)
        {_categoryRepository = categoryRepository;
        }
        // GET: api/<CategoryController>
        [HttpGet]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedComments = _categoryRepository.GetAllCategory(currentPage, 5);
            return Ok(pagedComments);
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(_categoryRepository.GetCategory(id));
        }

        // POST api/<CategoryController>
        [HttpPost]
        public IActionResult Post([FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _categoryRepository.CreateCategory(categoryDto);
            return Ok(categoryDto);
        }

        // PUT api/<CategoryController>/5
        [HttpPut]
        public IActionResult Put(CategoryDto categoryDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            _categoryRepository.UpdateCategory(categoryDto);
            return Ok(categoryDto);
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _categoryRepository.DeleteCategory(id);
            return Ok();
        }
    }
}
