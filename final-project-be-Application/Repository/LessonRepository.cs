using AutoMapper;
using CloudinaryDotNet;
using final_project_be_Application.Interface;
using final_project_be_Application.Service.CloudinaryService;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Lesson;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{
	public class LessonRepository : Repository<Lesson>, ILessonRepository
	{
		private readonly ILessonDAO _lessonDAO;
		private readonly ICloudinaryService _cloudinaryService;
		private readonly IMapper _mapper;
		private readonly ILogger<LessonRepository> _logger;
        private readonly IBlobStorageService _blobStorageService;
        public LessonRepository(ILessonDAO lessonDAO, ICloudinaryService cloudinaryService, IMapper mapper, ILogger<LessonRepository> logger, IBlobStorageService blobStorageService) : base(lessonDAO)
		{
			_lessonDAO = lessonDAO;
			_cloudinaryService = cloudinaryService;
			_mapper = mapper;
			_logger = logger;
			_blobStorageService = blobStorageService;
		}

		public async Task<Lesson> CreateLesson(LessonDto dto)
		{
			try
			{
				await _lessonDAO.BeginTransactionAsync();
				var lesson = _mapper.Map<Lesson>(dto);
				if (dto.Video != null && dto.Video.Length > 0)
				{
					var videoLink = await _cloudinaryService.UploadVideoAndGetUrlAsync(dto.Video);
					lesson.VideoLink = videoLink;
				}
                if (dto.Document != null && dto.Document.Length > 0)
                {
                    // Generate a unique filename using GUID
                    var fileExtension = Path.GetExtension(dto.Document.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    // Upload to Azure Blob Storage
                    using (var stream = dto.Document.OpenReadStream())
                    {
                        await _blobStorageService.UploadFileAsync(uniqueFileName, stream);
                    }

                    // Store the full URL in the database (UpdateAsync the blob URL with your storage account name)
                    lesson.DocumentLink = $"https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/{uniqueFileName}";
                }
                await _lessonDAO.AddAsync(lesson);
				await _lessonDAO.CommitTransactionAsync();
				_logger.LogInformation("Add Lesson success");
				return lesson;
			}
			catch (Exception ex)
			{
				await _lessonDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when adding Lesson");
				return null;
			}
		}

		public async Task<bool> DeleteLesson(int id)
		{
			try
			{
				await _lessonDAO.BeginTransactionAsync();
				await _lessonDAO.DeleteAsync(id);
				await _lessonDAO.CommitTransactionAsync();
				_logger.LogInformation("Delete Lesson success");
				return true;
			}
			catch (Exception ex)
			{
				await _lessonDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when delete Lesson");
				return false;
			}
		}

		public async Task<ICollection<LessonResponseDto>> GetAllLessonByModuleId(int moduleId)
		{
			try
			{
				await _lessonDAO.BeginTransactionAsync();

				var modules = _lessonDAO.GetAll()
					.Where(m => m.ModuleId == moduleId)
					.ToList();

				var moduleDtos = _mapper.Map<List<LessonResponseDto>>(modules);

				await _lessonDAO.CommitTransactionAsync();

				_logger.LogInformation("Successfully retrieved lessons for module ID {moduleId}", moduleId);
				return moduleDtos;
			}
			catch (Exception ex)
			{
				await _lessonDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error while retrieving lessons for module ID {moduleId}", moduleId);
				return new List<LessonResponseDto>();
			}
		}

		public async Task<Lesson> GetLesson(int id)
		{
			try
			{
				await _lessonDAO.BeginTransactionAsync();
				var lesson = await _lessonDAO.GetByIdAsync(id);
				await _lessonDAO.CommitTransactionAsync();
				_logger.LogInformation("Get Lesson success");
				return lesson;
			}
			catch (Exception ex)
			{
				await _lessonDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when getting Lesson");
				return null;
			}
		}

        public async Task<Lesson> UpdateLesson(UpdateLessonDto dto)
		{
			try
			{
				await _lessonDAO.BeginTransactionAsync();
				var lesson = await _lessonDAO.GetByIdAsync(dto.LessonId);
				if (lesson == null)
				{
					_logger.LogWarning("Lesson not found with ID: {Id}", dto.LessonId);
					await _lessonDAO.RollbackTransactionAsync();
					return null;
				}
				_mapper.Map(dto, lesson);
				var oldVideo = lesson.VideoLink;
                string oldDocumentUrl = lesson.DocumentLink;
                if (dto.Video != null && dto.Video.Length > 0)
				{
					if (!string.IsNullOrEmpty(oldVideo))
					{
						var deleted = await _cloudinaryService.DeleteVideoByUrlAsync(oldVideo);
					}
					var videoLink = await _cloudinaryService.UploadVideoAndGetUrlAsync(dto.Video);
					lesson.VideoLink = videoLink;
				}
                if (dto.Document != null && dto.Document.Length > 0)
                {
                    if (!string.IsNullOrEmpty(oldDocumentUrl))
                    {
                        var oldFileName = Path.GetFileName(oldDocumentUrl);
                        await _blobStorageService.DeleteFileIfExistsAsync(oldFileName);
                    }

                    var fileExtension = Path.GetExtension(dto.Document.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    using (var stream = dto.Document.OpenReadStream())
                    {
                        await _blobStorageService.UploadFileAsync(uniqueFileName, stream);
                    }

                    lesson.DocumentLink = $"https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/{uniqueFileName}";
                }
                await _lessonDAO.UpdateAsync(lesson);
				await _lessonDAO.CommitTransactionAsync();
				_logger.LogInformation("Update Lesson success");
				return lesson;
			}
			catch (Exception ex)
			{
				await _lessonDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when updating Lesson");
				return null;
			}
		}
	}
}
