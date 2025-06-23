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

        public Caculator(LessonDAO lessonDAO)
        {
            _lessonDAO = lessonDAO;
        }

        public double CalculateModuleCompletion(Guid userId, int moduleId)
        {
            var totalLessons = _lessonDAO.CountLessonsInModule(moduleId);
            if (totalLessons == 0) return 0;

            var completedLessons = _lessonDAO.CountCompletedLessonsInModule(userId, moduleId);
            return (double)completedLessons / totalLessons * 100;
        }

        public double CalculateCourseCompletion(Guid userId, int courseId)
        {
            var moduleIds = _lessonDAO.GetModuleIdsByCourseId(courseId);
            if (!moduleIds.Any()) return 0;

            double totalCompletion = 0;
            foreach (var moduleId in moduleIds)
            {
                totalCompletion += CalculateModuleCompletion(userId, moduleId);
            }

            return totalCompletion / moduleIds.Count;
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

        public double CalculateModuleProgress(Guid userId, int moduleId)
        {
            int totalLessons = _lessonDAO.CountLessonsInModule(moduleId);
            if (totalLessons == 0) return 0;

            int completedLessons = _lessonDAO.CountCompletedLessonsInModule(userId, moduleId);
            return (double)completedLessons / totalLessons * 100;
        }
    }
}
