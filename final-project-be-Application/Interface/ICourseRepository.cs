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
		public PageResult<GetCourseDto> GetAllCourses(int page, int pageSize, int? CategoryId, string? title, Guid? userId, string? sortOption, int? mentorId, string? Language, string? Level, decimal? MinCost, decimal? MaxCost, decimal? MinRate, decimal? MaxRate, List<StatusEnum>? statuses);
        public Task<List<UserCourseDto>> GetUserCoursesAsync(Guid userId);
		public Task<List<UserCourseDto>> GetUserCoursesByStatusAsync(Guid userId, string? status);
        public Task<GetCourseDto> ToggleIsDeleted(int id);
        public Task<GetCourseDto?> ToggleStatus(int id, string statuses);
		public Task<List<CourseRecommendationDto>> RecommendCoursesAsync(Guid userId);
		public Task<List<MonthlyStatCourseDto>> GetStatisticsByMonth(Guid userId, int? year);

    }
}
