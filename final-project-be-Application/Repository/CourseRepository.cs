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
        private readonly UserCourseDAO _userCourseDAO;
		private readonly ReviewDAO _reviewDAO;
        private readonly Caculator _Caculator;
        private readonly IMapper _mapper;
		private readonly ILogger<CourseRepository> _logger;
        private readonly IBlobStorageService _blobStorageService;
 
		public CourseRepository(CourseDAO courseDAO, Caculator Caculator,UserCourseDAO userCourseDAO, ReviewDAO reviewDAO, IMapper mapper, ILogger<CourseRepository> logger, IBlobStorageService blobStorageService) : base(courseDAO)

		{
			_courseDAO = courseDAO;
            _Caculator = Caculator;
            _reviewDAO = reviewDAO;
            _userCourseDAO = userCourseDAO;
			_mapper = mapper;
			_logger = logger;
			_blobStorageService = blobStorageService;
		}

		public async Task<Courses> CreateCourse(CourseDto dto)
		{
			try
			{
				await _courseDAO.BeginTransactionAsync();
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

					// Store the full URL in the database (UpdateAsync the blob URL with your storage account name)
					course.CoursesImage = $"https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/{uniqueFileName}";
				}
				course.CreateAt = DateTime.Now;
				course.UpdateAt = DateTime.Now;
				course.StudentCount = 0;
                course.Status = "Not Completed";
				await _courseDAO.AddAsync(course);
				await _courseDAO.CommitTransactionAsync();
				_logger.LogInformation("AddAsync Course success");
				return course;

			}
			catch (Exception ex)
			{
				await _courseDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when adding Course");
				return null;
			}
		}



        public PageResult<GetCourseDto> GetAllCourses(int page, int pageSize, int? CategoryId, string? title, Guid? userId, string? sortOption, int? mentorId, string? Language, string? Level, decimal? MinCost, decimal? MaxCost, decimal? MinRate, decimal? MaxRate, List<StatusEnum>? statuses)
        {
            try
            {
                var query = _courseDAO.GetAll()
                    .Include(c => c.Mentor)
                        .ThenInclude(c => c.User)
                            .ThenInclude(c => c.UserMetaData)
                    .Include(c => c.Category)
                    .Include(c => c.Reviews.Where(r => !r.IsDeleted))
                    .Where(p => !p.IsDeleted && (statuses == null || statuses.Count == 0 || statuses.Select(s => s.ToString()).Contains(p.Status)));


                if (CategoryId.HasValue)
                    query = query.Where(c => c.CategoryId == CategoryId);

                if (mentorId.HasValue)
                    query = query.Where(c => c.MentorId == mentorId);

                if (!string.IsNullOrEmpty(title))
                    query = query.Where(c => c.CourseName.Contains(title));

                if (userId.HasValue && userId != Guid.Empty)
                    query = query.Where(p => p.UserCourses.Any(uc => uc.UserId == userId.Value));


                if (!string.IsNullOrEmpty(Language))
                    query = query.Where(c => c.Language == Language);

                if (!string.IsNullOrEmpty(Level))
                    query = query.Where(c => c.Level == Level);

                if (MinCost.HasValue)
                    query = query.Where(c => c.Cost >= MinCost.Value);
                if (MaxCost.HasValue)
                    query = query.Where(c => c.Cost <= MaxCost.Value);

                if (MinRate.HasValue)
                    query = query.Where(c => c.Reviews.Any() && c.Reviews.Average(r => r.Rate) >= MinRate.Value);
                if (MaxRate.HasValue)
                    query = query.Where(c => c.Reviews.Any() && c.Reviews.Average(r => r.Rate) <= MaxRate.Value);

                query = sortOption?.ToLower() switch
                {
                    "asc_name" => query.OrderBy(c => c.CourseName),
                    "desc_name" => query.OrderByDescending(c => c.CourseName),
                    "asc_date" => query.OrderBy(c => c.CreateAt),
                    "desc_date" => query.OrderByDescending(c => c.CreateAt),
                    "asc_cost" => query.OrderBy(c => c.Cost),
                    "desc_cost" => query.OrderByDescending(c => c.Cost),
                    "asc_rating" => query.OrderBy(c => c.Reviews.Any() ? c.Reviews.Average(r => r.Rate) : 0),
                    "desc_rating" => query.OrderByDescending(c => c.Reviews.Any() ? c.Reviews.Average(r => r.Rate) : 0),
                    "most_reviewed" => query.OrderByDescending(c => c.Reviews.Count()),
                    _ => query.OrderByDescending(c => c.CreateAt)
                };

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
                    CreateAt = p.CreateAt,
                    Language = p.Language,
                    Level = p.Level,
                    Status = p.Status,
                    AverageRating = p.Reviews.Any() ? Math.Round(p.Reviews.Average(r => r.Rate), 1) : 0,
                    TotalReviews = p.Reviews.Count(),
                    Mentor = new MentorDto
                    {
                        FirstName = p.Mentor.User.UserMetaData.FirstName,
                        LastName = p.Mentor.User.UserMetaData.LastName
                    }
                }).ToList();

                _logger.LogInformation("Get filtered courses success");
                return new PageResult<GetCourseDto>(coursesDtos, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting filtered courses");
                return new PageResult<GetCourseDto>(new List<GetCourseDto>(), 0, page, pageSize);
            }
        }

        public async Task<CourseResponseDto> GetCourse(int id)
		{
			try{
				await _courseDAO.BeginTransactionAsync();

				var course = await _courseDAO.GetByIdAsync(id);
				var courseDto = _mapper.Map<CourseResponseDto>(course);
				courseDto.CountModule = course.Modules?.Count ?? 0;
				courseDto.CountLesson = course.Modules?.Sum(m => m.Lessons?.Count ?? 0) ?? 0;

				await _courseDAO.CommitTransactionAsync();

				_logger.LogInformation("Get course success");
				return courseDto;
			}
			catch (Exception ex)
			{
				await _courseDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when getting course");
				return null;
			}
		}

        public async Task<List<UserCourseDto>> GetUserCoursesAsync(Guid userId)
        {
            var userCourses = await _userCourseDAO.GetUserCoursesByUserId(userId);

            var result = new List<UserCourseDto>();

            foreach (var uc in userCourses)
            {
                // 👉 Gọi lại hàm tính tiến độ để đảm bảo cập nhật mới nhất
                var updatedPercentage = await _Caculator.CalculateCourseCompletion(userId, uc.CourseId);

                result.Add(new UserCourseDto
                {
                    CourseId = uc.CourseId,
                    CourseName = uc.Courses?.CourseName ?? "Unknown",
                    CourseImage = uc.Courses?.CoursesImage,
                    CertificateLink = uc.CertificateLink,
                    Status = uc.Status,
                    Percentage = updatedPercentage,
                    CompletedAt = uc.CompletedAt
                });
            }

            return result;
        }

        public async Task<List<UserCourseDto>> GetUserCoursesByStatusAsync(Guid userId, string? status)
        {
            var userCourses = await _userCourseDAO.GetUserCoursesByUserId(userId);

            var result = new List<UserCourseDto>();

            foreach (var uc in userCourses)
            {
                // Gọi tính tiến độ để cập nhật lại dữ liệu mới nhất
                await _Caculator.CalculateCourseCompletion(userId, uc.CourseId);
                
                // Nếu có truyền status thì lọc
                if (string.IsNullOrEmpty(status) || string.Equals(uc.Status, status, StringComparison.OrdinalIgnoreCase))
                {
                    result.Add(new UserCourseDto
                    {
                        CourseId = uc.CourseId,
                        CourseName = uc.Courses?.CourseName ?? "Unknown",
                        CourseImage = uc.Courses?.CoursesImage,
                        CertificateLink = uc.CertificateLink,
                        Status = uc.Status,
                        Percentage = uc.Percentage,
                        CompletedAt = uc.CompletedAt
                    });
                }
            }

            return result;
        }

        public async Task<Courses> ToggleIsDeleted(int id)
		{
			try
			{
				await _courseDAO.BeginTransactionAsync();

				var course = await _courseDAO.GetByIdAsync(id);
				if (course == null)
				{
					_logger.LogWarning("Course not found with ID: {Id}", id);
					await _courseDAO.RollbackTransactionAsync();
					return null;
				}

				course.IsDeleted = !course.IsDeleted;
				course.UpdateAt = DateTime.Now;

				await _courseDAO.UpdateAsync(course);
				await _courseDAO.CommitTransactionAsync();

				_logger.LogInformation("Toggle IsDeleted success for course ID: {Id}", id);
				return course;
			}
			catch (Exception ex)
			{
				await _courseDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when toggling IsDeleted for course ID: {Id}", id);
				return null;
			}
		}

        public async Task<Courses> ToggleStatus(int id, string statuses)
        {
            try
            {
                await _courseDAO.BeginTransactionAsync();

                var course = await _courseDAO.GetByIdAsync(id);
                if (course == null)
                {
                    _logger.LogWarning("Course not found with ID: {Id}", id);
                    await _courseDAO.RollbackTransactionAsync();
                    return null;
                }

                course.Status = statuses;
                course.UpdateAt = DateTime.Now;

                await _courseDAO.UpdateAsync(course);
                await _courseDAO.CommitTransactionAsync();

                _logger.LogInformation("Toggle Status success for course ID: {Id}", id);
                return course;
            }
            catch (Exception ex)
            {
                await _courseDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when toggling Status for course ID: {Id}", id);
                return null;
            }
        }

        public async Task<Courses> UpdateCourse(UpdateCourseDto dto)
        {
            try
            {
                await _courseDAO.BeginTransactionAsync();

                var course = await _courseDAO.GetByIdAsync(dto.CourseId);
                if (course == null)
                {
                    _logger.LogWarning("Course not found with ID: {Id}", dto.CourseId);
                    await _courseDAO.RollbackTransactionAsync();
                    return null;
                }

                string oldImageUrl = course.CoursesImage;

                course.CourseName = dto.CourseName;
                course.CourseContent = dto.CourseContent;
                course.Cost = dto.Cost;
                course.SkillLearn = dto.SkillLearn;
                course.CourseLength = dto.CourseLength;
                course.CategoryId = dto.CategoryId;
                course.MentorId = dto.MentorId;
                course.IntendedLearner = dto.IntendedLearner;
                course.Level = dto.Level;
                course.Language = dto.Language;
                course.Requirement = dto.Requirement;

                course.Status = "Pending";

                if (dto.CoursesImage != null && dto.CoursesImage.Length > 0)
                {
                    if (!string.IsNullOrEmpty(oldImageUrl))
                    {
                        var oldFileName = Path.GetFileName(oldImageUrl);
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

                await _courseDAO.UpdateAsync(course);
                await _courseDAO.CommitTransactionAsync();
                _logger.LogInformation("UpdateAsync Course success");
                return course;
            }
            catch (Exception ex)
            {
                await _courseDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when updating Course");
                return null;
            }
        }

    }
}
