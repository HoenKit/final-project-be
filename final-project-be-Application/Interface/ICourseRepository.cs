using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Courses;

namespace final_project_be_Application.Interface
{
	public interface ICourseRepository : IRepository<Courses>
	{
		public Task<Courses> CreateCourse(CourseDto dto);
		public Task<CourseResponseDto> GetCourse(int id);
		public Task<Courses> UpdateCourse(UpdateCourseDto dto);
		public PageResult<GetCourseDto> GetAllCourses(int page, int pageSize, int? CategoryId, string? title, Guid? userId, string? sortOption, int? mentorId, string? Language, string? Level, decimal? MinCost, decimal? MaxCost, decimal? MinRate, decimal? MaxRate);

        public Task<Courses> ToggleIsDeleted(int id);
	}
}
