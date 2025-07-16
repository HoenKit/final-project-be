using AutoMapper;
using CloudinaryDotNet;
using final_project_be_Application.Interface;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Lesson;
using final_project_be_Domain.DTOs.Module;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{
    public class ModuleRepository : Repository<Module>, IModuleRepository
    {
        private readonly IModuleDAO _moduleDAO;
        private readonly IUserModuleDAO _userModuleDAO;
        private readonly Caculator _caculator;
        private readonly IUserLessonDAO _userlessonDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<ModuleRepository> _logger;
        private readonly IOpenAIEmbeddingService _embeddingService;
        private readonly ILessonRepository _lessonRepository;
        private readonly ICourseRepository _courseRepository;

        public ModuleRepository(IModuleDAO moduleDAO, IMapper mapper, ILogger<ModuleRepository> logger, IUserLessonDAO userlessonDAO, IUserModuleDAO userModuleDAO, Caculator caculator, IOpenAIEmbeddingService embeddingService, ILessonRepository lessonRepository, ICourseRepository courseRepository) : base(moduleDAO)
        {
            _userlessonDAO = userlessonDAO;
            _userModuleDAO = userModuleDAO;
            _caculator = caculator;
            _moduleDAO = moduleDAO;
            _mapper = mapper;
            _logger = logger;
            _embeddingService = embeddingService;
            _lessonRepository = lessonRepository;
            _courseRepository = courseRepository;
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
            var result = new List<ModuleProgressDto>();

            foreach (var module in modules)
            {
                // ✅ Cập nhật lại % và trạng thái trong UserModule
                float updatedPercentage = await _caculator.CalculateModuleProgress(userId, module.ModuleId);

                var userLessons = await _userlessonDAO.GetUserLessonsByModuleAsync(userId, module.ModuleId);

                var moduleDto = new ModuleProgressDto
                {
                    ModuleId = module.ModuleId,
                    Title = module.Title,
                    Description = module.Description,
                    Percentage = updatedPercentage,
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

                result.Add(moduleDto);
            }

            return result;
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
        public async Task<List<AIGeneratedModule>> GenerateModulesAndLessonsFromDescriptionAsync(string courseDescription)
        {
            var prompt = $@"
		You are an expert instructional designer. Based on the following course description, generate a list of modules, each with several lessons.

		Format the output strictly as a JSON array like this:
		[
		  {{
			""Title"": ""[Module title]"",
			""Description"": ""[Module description]"",
			""Lessons"": [
			  {{
				""Title"": ""[Lesson title]"",
				""Description"": ""[Lesson description]""
			  }}
			]
		  }}
		]

		Only return valid JSON. No explanations, no markdown.

		Course Description:
		{courseDescription}"
        ;

            try
            {
                var rawJson = await _embeddingService.GetChatCompletionAsync(prompt);

                // Clean up if AI wraps response in triple backticks
                var match = Regex.Match(rawJson, @"```(?:json)?\s*(.+?)\s*```", RegexOptions.Singleline);
                string cleanJson = match.Success ? match.Groups[1].Value : rawJson;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<List<AIGeneratedModule>>(cleanJson, options) ?? new List<AIGeneratedModule>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[OpenAI Error] {ex.Message}");
                return new List<AIGeneratedModule>();
            }
        }

        public async Task<bool> GenerateAndSaveModulesAsync(int courseId)
        {
            var course = await _courseRepository.GetCourse(courseId);
            var modules = await GenerateModulesAndLessonsFromDescriptionAsync(course.CourseContent);

            foreach (var module in modules)
            {
                var createdModule = await CreateModule(new ModuleDto
                {
                    CourseId = courseId,
                    Title = module.Title,
                    Description = module.Description,
                    IsPremium = false
                });

                foreach (var lesson in module.Lessons)
                {
                    await _lessonRepository.CreateLesson(new LessonDto
                    {
                        ModuleId = createdModule.ModuleId,
                        Title = lesson.Title,
                        Description = lesson.Description,
                        Document = null,
                        Video = null
                    });
                }
            }
            return true;
        }

    }
}
