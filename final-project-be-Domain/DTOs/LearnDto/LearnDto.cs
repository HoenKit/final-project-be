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

    public class UserAssignmentDto
    {
        public int AssignmentId { get; set; }
        public Guid UserId { get; set; }
        public string? Content { get; set; }
        public bool IsScored { get; set; }
        public bool IsPresented { get; set; }
        public DateTime? CreateAt { get; set; }
    }

    public class submitAssignmentDto
    {
        public Guid UserId { get; set; }
        public int AssignmentId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class GradeAssignmentDto
    {
        public Guid UserId { get; set; }
        public int LessonId { get; set; }
        public int AssignmentId { get; set; }
        public float Mark { get; set; }
        public bool IsPassed { get; set; }
    }



    public class MarkPresentDto
    {
        public int AssignmentId { get; set; }
        public List<Guid> UserIds { get; set; } = new();
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
        public string? QuestionType { get; set; }
        public List<QuizAnswer> Answers { get; set; }
    }
    public class SubmitQuizDto
    {
        public Guid UserId { get; set; }
        public int LessonId { get; set; }
        public List<int> AnswerIds { get; set; } = new();
    }
}
