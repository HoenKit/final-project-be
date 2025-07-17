using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.LearnDto;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{
    public class LearningRepository : ILearningRepository
    {
        private readonly IUserCourseDAO _usercourseDAO;
        private readonly IUserLessonDAO _userlessonDAO;
        private readonly IUserModuleDAO _userModuleDAO;
        private readonly IUserAnswerDAO _userAnswerDAO;
        private readonly IModuleDAO _moduleDAO;
        private readonly ILessonDAO _lessonDAO;
        private readonly IQuestionDAO _questionDAO;
        private readonly ICaculator _caculator;
        private readonly IMapper _mapper;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<LearningRepository> _logger;

        public LearningRepository(IUserCourseDAO usercourseDAO, IUserLessonDAO userlessonDAO,IBlobStorageService blobStorageService, IUserAnswerDAO userAnswerDAO, ILessonDAO lessonDAO, IUserModuleDAO userModuleDAO, IMapper mapper, ILogger<LearningRepository> logger,
            IModuleDAO moduleDAO,ICaculator caculator, IQuestionDAO questionDAO)
        {
            _lessonDAO = lessonDAO;
            _usercourseDAO = usercourseDAO;
            _userlessonDAO = userlessonDAO;
            _userModuleDAO = userModuleDAO;
            _userAnswerDAO = userAnswerDAO;
            _blobStorageService = blobStorageService;
            _mapper = mapper;
            _logger = logger;
            _moduleDAO = moduleDAO;
            _caculator = caculator;
            _questionDAO = questionDAO;
        }

        public async Task StartCourseAsync(Guid userId, int courseId)
        {

            if (!await _usercourseDAO.UserCourseExists(userId, courseId))
            {
                await _usercourseDAO.AddUserCourseAsync(new UserCourse
                {
                    UserId = userId,
                    CourseId = courseId,
                    CompletedAt = DateTime.UtcNow,
                    Status = "Pending",
                });
            }

            var modules = await _moduleDAO.GetModulesByCourseId(courseId);
            foreach (var module in modules)
            {
                if (!await _userModuleDAO.UserModuleExists(userId, module.ModuleId))
                {
                    await _userModuleDAO.AddUserModuleAsync(new UserModule
                    {
                        UserId = userId,
                        ModuleId = module.ModuleId,
                        CompletedAt = null
                    });
                }
            }

            await _userModuleDAO.SaveChangesAsync();
        }

        public async Task<UserLesson> CompleteLessonAsync(Guid userId, int lessonId, float? score)
        {
            var existing = await _userlessonDAO.GetUserLessonbyuserandlessonAsync(userId, lessonId);

            if (existing != null)
            {
                if (existing.IsPassed)
                    throw new InvalidOperationException("User has already completed this lesson.");

                // ❌ Nếu chưa pass thì xóa để thêm bản ghi mới
                await _userlessonDAO.DeleteUserLessonAsync(existing);
                await _userlessonDAO.SaveChangesAsync();
            }

            var lesson = await _lessonDAO.GetLessonByIdAsync(lessonId);
            if (lesson == null)
                throw new Exception("Lesson not found.");

            // Xác định loại bài học
            var isQuiz = await _lessonDAO.HasQuestionAsync(lessonId);
            var isAssignment = await _lessonDAO.HasAssignmentAsync(lessonId);
            var isDocs = !string.IsNullOrEmpty(lesson.DocumentLink);
            var isVideo = !string.IsNullOrEmpty(lesson.VideoLink);

            var userLesson = new UserLesson
            {
                UserId = userId,
                LessonId = lessonId,
                CompletedAt = DateTime.UtcNow
            };

            if (isQuiz)
            {
                if (score == null)
                    throw new ArgumentException("Score is required for quiz.");
                userLesson.Mark = score;
                userLesson.IsPassed = score >= 80;
            }
            else if (isAssignment)
            {
                userLesson.Mark = null;
                userLesson.IsPassed = false;
            }
            else
            {
                // ✅ Default: nếu không phải quiz, không phải assignment → luôn pass
                userLesson.Mark = 100;
                userLesson.IsPassed = true;
            }

            await _userlessonDAO.AddUserLessonAsync(userLesson);
            await _userlessonDAO.SaveChangesAsync();

            return userLesson;
        }

        public async Task<float> SubmitQuizAsync(SubmitQuizDto dto)
        {
            var lesson = await _lessonDAO.GetLessonByIdAsync(dto.LessonId);
            if (lesson == null)
                throw new Exception("Lesson not found");

            if (!await _lessonDAO.IsQuizLessonAsync(dto.LessonId))
                throw new Exception("This lesson is not a quiz.");

            var userLesson = await _lessonDAO.GetUserLessonAsync(dto.UserId, dto.LessonId);
            if (userLesson == null)
            {
                userLesson = new UserLesson
                {
                    UserId = dto.UserId,
                    LessonId = dto.LessonId,
                    CompletedAt = DateTime.UtcNow
                };
                await _userlessonDAO.AddUserLessonAsync(userLesson);
                await _lessonDAO.SaveChangesAsync();
            }

            // Xoá userAnswer cũ
            await _userAnswerDAO.DeleteUserAnswersByUserLessonIdAsync(userLesson.UserLessonId);
            await _lessonDAO.SaveChangesAsync();

            // Tạo mới
            var userAnswers = dto.AnswerIds
                .Select(aid => new UserAnswer
                {
                    UserLessonId = userLesson.UserLessonId,
                    AnswerId = aid
                }).ToList();

            await _lessonDAO.AddUserAnswersAsync(userAnswers);
            await _lessonDAO.SaveChangesAsync();

            var (score, isPassed) = await _caculator.CalculateQuizScore(dto.UserId, dto.LessonId);

            return score;
        }

        public async Task<List<QuizDto>> GetQuizByLessonIdAsync(int lessonId)
        {
            var questions = await _questionDAO.GetQuestionsByLessonIdAsync(lessonId);

            return questions.Select(q => new QuizDto
            {
                QuestionText = q.Question_text,
                QuestionType = q.QuestionType,
                Answers = q.Answers.Select(a => new QuizAnswer
                {
                    AnswerId = a.AnswerId,
                    Text = a.Text,
                    IsCorrect = a.Is_correct,
                }).ToList()
            }).ToList();
        }

        public async Task<bool> UploadCertificateAndSaveLinkAsync(CertificateUploadDto dto)
        {
            var userCourse = await _usercourseDAO.GetCompletedUserCourseAsync(dto.UserId, dto.CourseId);
            if (userCourse == null)
                return false;

            var fileName = $"certificates/User{dto.UserId}_Course{dto.CourseId}_{DateTime.UtcNow.Ticks}{Path.GetExtension(dto.CertificateFile.FileName)}";

            using var stream = dto.CertificateFile.OpenReadStream();
            await _blobStorageService.UploadFileAsync(fileName, stream);

            var fileUrl = $"https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/{fileName}";
            await _usercourseDAO.UpdateCertificateLinkAsync(dto.UserId, dto.CourseId, fileUrl);

            return true;
        }
    }
}
