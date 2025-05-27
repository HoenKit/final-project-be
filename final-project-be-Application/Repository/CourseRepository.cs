using AutoMapper;
using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Post;
using final_project_be_Application.Interface;
using Microsoft.Extensions.Logging;

namespace final_project_be_Application.Repository
{
	public class CourseRepository : Repository<Courses>, ICourseRepository
	{
		private readonly CourseDAO _courseDAO;
		private readonly IMapper _mapper;
		private readonly ILogger<CourseRepository> _logger;
		public CourseRepository(CourseDAO courseDAO, IMapper mapper, ILogger<CourseRepository> logger) : base(courseDAO)
		{
			_courseDAO = courseDAO;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<Courses> CreateCourse(CourseDto dto)
		{
			try
			{
				_courseDAO.BeginTransaction();
				var course = _mapper.Map<Courses>(dto);
				course.CreateAt = DateTime.Now;
				course.UpdateAt = DateTime.Now;
				course.StudentCount = 0;
				_courseDAO.Add(course);
				_courseDAO.CommitTransaction();
				_logger.LogInformation("Add Course success");
				return course;

			}
			catch (Exception ex)
			{
				_courseDAO.RollbackTransaction();
				_logger.LogError(ex, "Error when adding Course");
				return null;
			}
		}

		public bool DeleteCourse(int id)
		{
			throw new NotImplementedException();
		}

		public PageResult<Courses> GetAllCourses(int page, int pageSize, int? subCategoryId, string? title, Guid? userId)
		{
			throw new NotImplementedException();
		}

		public Task<Courses> GetCourse(int id)
		{
			throw new NotImplementedException();
		}

		public Task<Courses> ToggleIsDeleted(int id)
		{
			throw new NotImplementedException();
		}

		public async Task<Courses> UpdateCourse(UpdateCourseDto dto)
		{
			try
			{
				_courseDAO.BeginTransaction();
				var course = _mapper.Map<Courses>(dto);
				course.UpdateAt = DateTime.Now;
				_courseDAO.Update(course);
				_courseDAO.CommitTransaction();
				_logger.LogInformation("Update Course success");
				return course;

			}
			catch (Exception ex)
			{
				_courseDAO.RollbackTransaction();
				_logger.LogError(ex, "Error when updating Course");
				return null;
			}
		}
	}
}
