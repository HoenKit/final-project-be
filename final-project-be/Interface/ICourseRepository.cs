using final_project_be.Data.Models;
using final_project_be.Dtos;

namespace final_project_be.Interface
{
	public interface ICourseRepository : IRepository<Courses>
	{
		public Task<Courses> CreateCourse(Courses dto);
		public bool DeleteCourse(int id);
		public Task<Courses> GetCourse(int id);
		public Task<Courses> UpdateCourse(Courses dto);
		public PageResult<Courses> GetAllCourses(int page, int pageSize, int? subCategoryId, string? title, Guid? userId);
		public Task<Courses> ToggleIsDeleted(int id);
	}
}
