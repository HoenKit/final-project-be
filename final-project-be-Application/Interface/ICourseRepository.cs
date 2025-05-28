using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Courses;

namespace final_project_be_Application.Interface
{
	public interface ICourseRepository : IRepository<Courses>
	{
		public Task<Courses> CreateCourse(CourseDto dto);
		public bool DeleteCourse(int id);
		public Task<Courses> GetCourse(int id);
		public Task<Courses> UpdateCourse(UpdateCourseDto dto);
		public PageResult<GetCourseDto> GetAllCourses(int page, int pageSize, int? CategoryId, string? title, Guid? userId);
		public Task<Courses> ToggleIsDeleted(int id);
	}
}
