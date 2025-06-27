using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.LearnDto;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
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
        private readonly UserCourseDAO _usercourseDAO;
        private readonly UserLessonDAO _userlessonDAO;
        private readonly UserModuleDAO _userModuleDAO;
        private readonly ModuleDAO _moduleDAO;
        private readonly LessonDAO _lessonDAO;
        private readonly QuestionDAO _questionDAO;
        private readonly Caculator _caculator;
        private readonly IMapper _mapper;
        private readonly ILogger<LearningRepository> _logger;

        public LearningRepository(UserCourseDAO usercourseDAO, UserLessonDAO userlessonDAO, LessonDAO lessonDAO, UserModuleDAO userModuleDAO, IMapper mapper, ILogger<LearningRepository> logger,
            ModuleDAO moduleDAO,Caculator caculator, QuestionDAO questionDAO)
        {
            _lessonDAO = lessonDAO;
            _usercourseDAO = usercourseDAO;
            _userlessonDAO = userlessonDAO;
            _userModuleDAO = userModuleDAO;
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
            if (await _userlessonDAO.UserLessonExists(userId, lessonId))
                throw new InvalidOperationException("User has already completed this lesson.");

            var lesson = await _lessonDAO.GetLessonByIdAsync(lessonId);
            if (lesson == null)
                throw new Exception("Lesson not found.");

            // Nhận diện loại bài học
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
            else if (isDocs || isVideo)
            {
                userLesson.Mark = 100;
                userLesson.IsPassed = true;
            }
            else
            {
                throw new Exception("Cannot determine lesson type. Missing links or associations.");
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

            // Tạo các UserAnswer
            var userAnswers = dto.AnswerIds.Select(aid => new UserAnswer
            {
                UserLessonId = userLesson.UserLessonId,
                AnswerId = aid
            }).ToList();

            await _lessonDAO.AddUserAnswersAsync(userAnswers);
            await _lessonDAO.SaveChangesAsync();

            // Gọi lại hàm tính điểm (hàm bạn đã có)
            var (score, isPassed) = await _caculator.CalculateQuizScore(dto.UserId, dto.LessonId);

            return score;
        }
        public async Task<List<QuizDto>> GetQuizByLessonIdAsync(int lessonId)
        {
            var questions = await _questionDAO.GetQuestionsByLessonIdAsync(lessonId);

            return questions.Select(q => new QuizDto
            {
                QuestionText = q.Question_text,
                Answers = q.Answers.Select(a => new QuizAnswer
                {
                    Text = a.Text,
                    IsCorrect = a.Is_correct,
                }).ToList()
            }).ToList();
        }

    }
}
