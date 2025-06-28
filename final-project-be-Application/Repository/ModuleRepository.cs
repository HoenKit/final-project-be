using AutoMapper;
using CloudinaryDotNet;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Lesson;
using final_project_be_Domain.DTOs.Module;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{
	public class ModuleRepository : Repository<Module>, IModuleRepository
	{
		private readonly ModuleDAO _moduleDAO;
		private readonly UserModuleDAO _userModuleDAO;
        private readonly UserLessonDAO _userlessonDAO;
        private readonly IMapper _mapper;
		private readonly ILogger<ModuleRepository> _logger;
		
		public ModuleRepository(ModuleDAO moduleDAO, IMapper mapper, ILogger<ModuleRepository> logger, UserLessonDAO userlessonDAO,UserModuleDAO userModuleDAO) : base(moduleDAO)
		{
            _userlessonDAO = userlessonDAO;
			_userModuleDAO = userModuleDAO;
            _moduleDAO = moduleDAO;
			_mapper = mapper;
			_logger = logger;
		}

		public async Task<Module> CreateModule(ModuleDto dto)
		{
			try
			{
				await _moduleDAO.BeginTransactionAsync();
				var module = _mapper.Map<Module>(dto);
				await _moduleDAO.AddAsync(module);
				await _moduleDAO.CommitTransactionAsync();
				_logger.LogInformation("AddAsync Module success");
				return module;
			}
			catch (Exception ex)
			{
				await _moduleDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when adding Module");
				return null;
			}
		}
        public async Task<List<ModuleProgressDto>> GetModuleProgressByCourseAsync(Guid userId, int courseId)
        {
            var modules = await _moduleDAO.GetModulesWithLessonsByCourseIdAsync(courseId);
            var userModules = await _userModuleDAO.GetUserModulesAsync(userId);
            var userLessons = await _userlessonDAO.GetUserLessonsAsync(userId);

            return modules.Select(module =>
            {
                var userModule = userModules.FirstOrDefault(um => um.ModuleId == module.ModuleId);

                return new ModuleProgressDto
                {
                    ModuleId = module.ModuleId,
                    Title = module.Title,
					Description = module.Description,
                    Percentage = userModule?.Percentage ?? 0,
                    Lessons = module.Lessons.Select(lesson =>
                    {
                        var userLesson = userLessons.FirstOrDefault(ul => ul.LessonId == lesson.LessonId);
                        return new LessonProgressDto
                        {
                            LessonId = lesson.LessonId,
                            Title = lesson.Title,
                            Ispassed = userLesson?.IsPassed ?? false
                        };
                    }).ToList()
                };
            }).ToList();
        }


        public async Task<bool> DeleteModule(int id)
		{
			try
			{
				await _moduleDAO.BeginTransactionAsync();
				await _moduleDAO.DeleteAsync(id);
				await _moduleDAO.CommitTransactionAsync();
				_logger.LogInformation("Delete Module success");
				return true;
			}
			catch (Exception ex)
			{
				await _moduleDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when delete Module");
				return false;
			}
		}
		public async Task<ICollection<ModuleResponseDto>> GetAllModulesByCourseId(int courseId)
		{
			try
			{
				await _moduleDAO.BeginTransactionAsync();

				var modules = _moduleDAO.GetAll()
					.Where(m => m.CourseId == courseId)
					.Include(m => m.Lessons)
					.ToList();

				var moduleDtos = _mapper.Map<List<ModuleResponseDto>>(modules);

				for (int i = 0; i < moduleDtos.Count; i++)
				{
					moduleDtos[i].CountLesson = modules[i].Lessons?.Count ?? 0;
				}

				await _moduleDAO.CommitTransactionAsync();

				_logger.LogInformation("Successfully retrieved modules for course ID {CourseId}", courseId);
				return moduleDtos;
			}
			catch (Exception ex)
			{
				await _moduleDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error while retrieving modules for course ID {CourseId}", courseId);
				return new List<ModuleResponseDto>();
			}
		}

		public async Task<ModuleResponseDto> GetModule(int id)
		{
			try
			{
				await _moduleDAO.BeginTransactionAsync();
				var module = await _moduleDAO.GetByIdAsync(id);
				var moduleDto = _mapper.Map<ModuleResponseDto>(module);
				moduleDto.CountLesson = module.Lessons?.Count ?? 0;
				await _moduleDAO.CommitTransactionAsync();
				_logger.LogInformation("Get Module success");
				return moduleDto;
			}
			catch (Exception ex)
			{
				await _moduleDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when getting Module");
				return null;
			}
		}

		public async Task<Module> UpdateModule(UpdateModuleDto dto)
		{
			try
			{
				await _moduleDAO.BeginTransactionAsync();
				var module = await _moduleDAO.GetByIdAsync(dto.ModuleId);
				if (module == null)
				{
					_logger.LogWarning("Module not found with ID: {Id}", dto.ModuleId);
					await _moduleDAO.RollbackTransactionAsync();
					return null;
				}
				_mapper.Map(dto, module);
				await _moduleDAO.UpdateAsync(module);
				await _moduleDAO.CommitTransactionAsync();
				_logger.LogInformation("UpdateAsync Module success");
				return module;
			}
			catch (Exception ex)
			{
				await _moduleDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when updating Module");
				return null;
			}
		}
	}
}
