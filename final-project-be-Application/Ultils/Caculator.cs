using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Module;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Ultils
{
    public class Caculator
    {
        private readonly LessonDAO _lessonDAO;
        private readonly ModuleDAO _moduleDAO;
        private readonly UserModuleDAO _userModuleDAO;
        private readonly UserCourseDAO _userCourseDAO;

        public Caculator(LessonDAO lessonDAO, ModuleDAO moduleDAO, UserCourseDAO userCourseDAO, UserModuleDAO userModuleDAO)
        {
            _lessonDAO = lessonDAO;
            _moduleDAO = moduleDAO;
            _userCourseDAO = userCourseDAO;
            _userModuleDAO = userModuleDAO;
        }

        public async Task<float> CalculateModuleCompletion(Guid userId, int moduleId)
        {
            var lessons = await _lessonDAO.GetLessonsByModuleId(moduleId);
            if (lessons == null || !lessons.Any()) return 0;

            var lessonIds = lessons.Select(l => l.LessonId).ToList();
            var passedLessons = (await _lessonDAO.GetUserPassedLessons(userId, lessonIds)).Count;

            return (float)passedLessons / lessonIds.Count * 100;
        }


        public async Task<float> CalculateCourseCompletion(Guid userId, int courseId)
        {
            var moduleIds = await _lessonDAO.GetModuleIdsByCourseId(courseId);
            if (!moduleIds.Any()) return 0;

            float totalCompletion = 0;
            foreach (var moduleId in moduleIds)
            {
                totalCompletion += await CalculateModuleCompletion(userId, moduleId); 
            }

            var percentage = totalCompletion / moduleIds.Count;

            // Cập nhật trạng thái vào bảng UserCourses
            var userCourse = await _userCourseDAO.GetUserCourse(userId, courseId);
            if (userCourse != null)
            {
                if (percentage == 100)
                {
                    userCourse.Status = "Completed";
                    userCourse.CompletedAt = DateTime.UtcNow;
                }
                else if (percentage > 0)
                {
                    userCourse.Status = "Pending";
                    userCourse.CompletedAt = DateTime.UtcNow;
                }
                else
                {
                    userCourse.Status = "NotStarted";
                    userCourse.CompletedAt = DateTime.UtcNow;
                }

                userCourse.Percentage = percentage;
                await _userCourseDAO.UpdateUserCourse(userCourse);
                // method để save lại
            }

            return percentage;
        }

        public async Task<(float Score, bool IsPassed)> CalculateQuizScore(Guid userId, int lessonId)
        {
            var isQuiz = await _lessonDAO.IsQuizLessonAsync(lessonId);
            if (!isQuiz)
                throw new InvalidOperationException("This lesson is not a quiz.");

            // Tìm userLessonId của user cho lesson đó
            var userLesson = await _lessonDAO.GetUserLessonAsync(userId, lessonId);
            if (userLesson == null)
                throw new Exception("UserLesson not found.");

            // Lấy danh sách câu trả lời
            var answers = await _lessonDAO.GetUserAnswersWithDetailsAsync(userLesson.UserLessonId);
            if (!answers.Any()) return (0, false);

            int totalQuestions = answers.Count;
            int correctAnswers = answers.Count(a => a.Answer.Is_correct);

            float score = 100f * correctAnswers / totalQuestions;
            bool isPassed = score >= 80;

            userLesson.Mark = score;
            userLesson.IsPassed = isPassed;
            await _lessonDAO.SaveChangesAsync();

            return (score, isPassed);
        }

        public async Task<float> CalculateModuleProgress(Guid userId, int moduleId)
        {
            int totalLessons = await _lessonDAO.CountLessonsInModule(moduleId);
            if (totalLessons == 0) return 0;

            int completedLessons = await _lessonDAO.CountCompletedLessonsInModule(userId, moduleId);
            float percentage = (float)completedLessons / totalLessons * 100;

            // Cập nhật trạng thái trong bảng UserModules
            var userModule = await  _userModuleDAO.GetUserModule(userId, moduleId);
            if (userModule != null)
            {
                if (percentage == 100)
                {
                    userModule.Status = "Completed";
                    userModule.CompletedAt = DateTime.UtcNow;
                }
                else if (percentage > 0)
                {
                    userModule.Status = "Pending";
                    userModule.CompletedAt = null;
                }
                else
                {
                    userModule.Status = "NotStarted";
                    userModule.CompletedAt = null;
                }

                userModule.Percentage = percentage;
                await  _userModuleDAO.UpdateUserModule(userModule); // DAO lưu lại thay đổi
            }

            return percentage;
        }
    }
}
