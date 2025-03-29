using final_project_be.Dtos.Category;
using final_project_be.Interface;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace final_project_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryRepository _subCategoryRepository;
        public SubCategoryController(ISubCategoryRepository subCategoryRepository)
        {
            _subCategoryRepository = subCategoryRepository;
        }
        // GET: api/<CategoryController>
        [HttpGet]
        public IActionResult GetAll(int? page)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            int currentPage = page ?? 1;
            var pagedComments = _subCategoryRepository.GetAllSubCategories(currentPage, 5);
            return Ok(pagedComments);
        }

        // GET api/<CategoryController>/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(_subCategoryRepository.GetSubCategory(id));
        }

        // POST api/<CategoryController>
        [HttpPost]
        public IActionResult Post([FromBody] SubCategoryDto subcategoryDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _subCategoryRepository.CreateSubCategory(subcategoryDto);
            return Ok(subcategoryDto);
        }

        // PUT api/<CategoryController>/5
        [HttpPut]
        public IActionResult Put(SubCategoryDto subcategoryDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }
            _subCategoryRepository.UpdateSubCategory(subcategoryDto);
            return Ok(subcategoryDto);
        }

        // DELETE api/<CategoryController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            _subCategoryRepository.DeleteSubCategory(id);
            return Ok();
        }
    }
}
