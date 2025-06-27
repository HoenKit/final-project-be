using final_project_be_Domain.DTOs.Answer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Domain.DTOs.LearnDto
{
    public class LearnDto
    {
    }
    public class StartCourseDto
    {
        public Guid UserId { get; set; }
        public int CourseId { get; set; }
    }

    public class UserLessonDto
    {
        public Guid UserId { get; set; }
        public int LessonId { get; set; }
        public DateTime CompletedAt { get; set; }
        public float? Mark { get; set; }
        public bool IsPassed { get; set; }
    }

    public class QuizDto
    {
        public string? QuestionText { get; set; }
        public List<QuizAnswer> Answers { get; set; }
    }
    public class SubmitQuizDto
    {
        public Guid UserId { get; set; }
        public int LessonId { get; set; }
        public List<int> AnswerIds { get; set; } = new();
    }
}
