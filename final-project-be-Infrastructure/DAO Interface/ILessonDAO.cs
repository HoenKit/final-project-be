using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface ILessonDAO : IGenericDAO<Lesson>
    {
        Task<int> GetCourseIdByLessonIdAsync(int lessonId);
        Task<int> CountLessonsInModule(int moduleId);
        Task<int> CountCompletedLessonsInModule(Guid userId, int moduleId);
        Task<List<int>> GetModuleIdsByCourseId(int courseId);
        Task<List<UserAnswer>> GetUserAnswersWithDetailsAsync(int userLessonId);
        Task<Lesson?> GetLessonByIdAsync(int lessonId);
        Task<List<Lesson>> GetLessonsByModuleId(int moduleId);
        Task<bool> IsQuizLessonAsync(int lessonId);
        Task<bool> HasQuestionAsync(int lessonId);
        Task<bool> HasAssignmentAsync(int lessonId);
        Task<List<UserLesson>> GetUserPassedLessons(Guid userId, List<int> lessonIds);
        Task<UserLesson?> GetUserLessonAsync(Guid userId, int lessonId);
        Task AddUserAnswersAsync(List<UserAnswer> userAnswers);
    }

}
