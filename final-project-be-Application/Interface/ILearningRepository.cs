using final_project_be_Domain.DTOs.LearnDto;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface ILearningRepository
    {
        public Task StartCourseAsync(Guid userId, int courseId);
        public Task<UserLesson> CompleteLessonAsync(Guid userId, int lessonId, float? score);
        public Task<float> SubmitQuizAsync(SubmitQuizDto dto);
        public Task<List<QuizDto>> GetQuizByLessonIdAsync(int lessonId);
    }

}
