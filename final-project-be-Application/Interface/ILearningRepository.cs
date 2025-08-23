using final_project_be_Domain.DTOs.Courses;
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
        public Task<UserCourse?> GetUserCourseAsync(Guid userId, int courseId);
        public Task<string?> UploadCertificateAndSaveLinkAsync(CertificateUploadDto dto);
        public Task<UserAssignment> CreateUserAssignmentAsync(CreateUserAssignmentDto dto);
        public Task<List<UserAssignmentInfoDto>> ListAssignmentsNotPresentAsync(int assignmentId);
        public Task<List<UserAssignmentDto>> GetUserAssignmentsByAssignmentIdAsync(int assignmentId);
        public Task<UserAssignmentDto?> GetUserAssignmentAsync(Guid userId, int assignmentId);
        public Task<bool> UpdateUserAssignmentAsync(submitAssignmentDto dto);
        public Task<bool> MarkUsersAsPresentAsync(MarkPresentDto dto);
        public Task<bool> GradeSubmissionAsync(GradeAssignmentDto grade);
    }

}
