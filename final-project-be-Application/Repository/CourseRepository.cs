using AutoMapper;
using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Post;
using final_project_be_Application.Interface;
using Microsoft.Extensions.Logging;
using final_project_be_Application.Ultils;
using Microsoft.EntityFrameworkCore;
using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.DTOs.Mentor;
using Azure;

namespace final_project_be_Application.Repository
{
	public class CourseRepository : Repository<Courses>, ICourseRepository
	{
		private readonly CourseDAO _courseDAO;
		private readonly IMapper _mapper;
		private readonly ILogger<CourseRepository> _logger;
		private readonly BlobStorageService _blobStorageService;
		public CourseRepository(CourseDAO courseDAO, IMapper mapper, ILogger<CourseRepository> logger, BlobStorageService blobStorageService) : base(courseDAO)
		{
			_courseDAO = courseDAO;
			_mapper = mapper;
			_logger = logger;
			_blobStorageService = blobStorageService;
		}

		public async Task<Courses> CreateCourse(CourseDto dto)
		{
			try
			{
				_courseDAO.BeginTransaction();
				var course = _mapper.Map<Courses>(dto);
				if (dto.CoursesImage != null && dto.CoursesImage.Length > 0)
				{
					// Generate a unique filename using GUID
					var fileExtension = Path.GetExtension(dto.CoursesImage.FileName);
					var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

					// Upload to Azure Blob Storage
					using (var stream = dto.CoursesImage.OpenReadStream())
					{
						await _blobStorageService.UploadFileAsync(uniqueFileName, stream);
					}

					// Store the full URL in the database (update the blob URL with your storage account name)
					course.CoursesImage = $"https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/{uniqueFileName}";
				}
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

		public PageResult<GetCourseDto> GetAllCourses(int page, int pageSize, int? CategoryId, string? title, Guid? userId)
		{
			try
			{
				var baseQuery = _courseDAO.GetAll()
					.Include(c => c.Mentor)
						.ThenInclude(c => c.User)
							.ThenInclude(c => c.UserMetaData)
					.Include(c => c.Category)
					.OrderByDescending(p => p.CreateAt);

				var query = baseQuery.Where(p => p.IsDeleted == false);
				if (CategoryId != null)
				{
					query = query.Where(c => c.CategoryId == CategoryId);
				}

				if (!string.IsNullOrEmpty(title))
				{
					query = query.Where(c => c.CourseName.Contains(title));
				}

				if (userId.HasValue && userId != Guid.Empty)
				{
					query = query.Where(p => p.UserCourses.Any(uc => uc.UserId == userId.Value));
				}

				var totalCount = query.Count();

				var courses = query
					.Skip((page - 1) * pageSize)
					.Take(pageSize)
					.ToList();
				var coursesDtos = courses.Select(p => new GetCourseDto
				{
					CourseId = p.CourseId,
					CourseName = p.CourseName,
					CourseContent = p.CourseContent,
					Cost = p.Cost,
					SkillLearn = p.SkillLearn,
					StudentCount = p.StudentCount,
					CoursesImage = p.CoursesImage,
					CourseLength = p.CourseLength,

					Mentor =  new MentorDto
					{
						FirstName = p.Mentor.User.UserMetaData.FirstName,
						LastName = p.Mentor.User.UserMetaData.LastName
					},

				}).ToList();
				_logger.LogInformation("Get Courses success");

				return new PageResult<GetCourseDto>(coursesDtos, totalCount, page, pageSize);
			}

			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when getting Posts");
				return new PageResult<GetCourseDto>(new List<GetCourseDto>(), 0, page, pageSize);
			}
		}

		public async Task<Courses> GetCourse(int id)
		{
			try{
				_courseDAO.BeginTransaction();
				var course = _courseDAO.GetById(id);
				_courseDAO.CommitTransaction();

				_logger.LogInformation("Get course success");
				return course;
			}
			catch (Exception ex)
			{
				_courseDAO.RollbackTransaction();
				_logger.LogError(ex, "Error when getting course");
				return null;
			}
		}

		public async Task<Courses> ToggleIsDeleted(int id)
		{
			try
			{
				_courseDAO.BeginTransaction();

				var course = _courseDAO.GetById(id);
				if (course == null)
				{
					_logger.LogWarning("Course not found with ID: {Id}", id);
					_courseDAO.RollbackTransaction();
					return null;
				}
				course.IsDeleted = !course.IsDeleted;
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

		public async Task<Courses> UpdateCourse(UpdateCourseDto dto)
		{
			try
			{
				_courseDAO.BeginTransaction();

				var course = _courseDAO.GetById(dto.CourseId);
				if (course == null)
				{
					_logger.LogWarning("Course not found with ID: {Id}", dto.CourseId);
					_courseDAO.RollbackTransaction();
					return null;
				}

				string oldImage = course.CoursesImage;
				_mapper.Map(dto, course);

				if (dto.CoursesImage != null && dto.CoursesImage.Length > 0)
				{
					if (!string.IsNullOrEmpty(oldImage))
					{
						var oldFileName = Path.GetFileName(oldImage);
						await _blobStorageService.DeleteFileIfExistsAsync(oldFileName);
					}

					var fileExtension = Path.GetExtension(dto.CoursesImage.FileName);
					var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

					using (var stream = dto.CoursesImage.OpenReadStream())
					{
						await _blobStorageService.UploadFileAsync(uniqueFileName, stream);
					}

					course.CoursesImage = $"https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/{uniqueFileName}";
				}

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
