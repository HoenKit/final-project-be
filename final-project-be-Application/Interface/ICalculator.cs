using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface ICaculator
    {
        Task<float> CalculateModuleCompletion(Guid userId, int moduleId);
        Task<float> CalculateCourseCompletion(Guid userId, int courseId);
        Task<(float Score, bool IsPassed)> CalculateQuizScore(Guid userId, int lessonId);
        Task<float> CalculateModuleProgress(Guid userId, int moduleId);
        double CosineSimilarity(float[] a, float[] b);
    }
}
