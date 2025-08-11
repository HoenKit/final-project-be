using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Assignment;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.LearnDto;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.EntityFrameworkCore;
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
        private readonly IUserAssignmentDAO _userAssignmentDAO;
        private readonly IAssignmentDAO _assignmentDAO;
        private readonly IModuleDAO _moduleDAO;
        private readonly ILessonDAO _lessonDAO;
        private readonly IQuestionDAO _questionDAO;
        private readonly ICaculator _caculator;
        private readonly IMapper _mapper;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ILogger<LearningRepository> _logger;

        public LearningRepository(IUserCourseDAO usercourseDAO, IUserAssignmentDAO userAssignmentDAO, IUserLessonDAO userlessonDAO,IBlobStorageService blobStorageService, IUserAnswerDAO userAnswerDAO, ILessonDAO lessonDAO, IUserModuleDAO userModuleDAO, IMapper mapper, ILogger<LearningRepository> logger,
            IModuleDAO moduleDAO,IAssignmentDAO assignmentDAO,ICaculator caculator, IQuestionDAO questionDAO)
        {
            _lessonDAO = lessonDAO;
            _usercourseDAO = usercourseDAO;
            _assignmentDAO = assignmentDAO;
            _userlessonDAO = userlessonDAO;
            _userModuleDAO = userModuleDAO;
            _userAnswerDAO = userAnswerDAO;
            _blobStorageService = blobStorageService;
            _userAssignmentDAO = userAssignmentDAO;
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

            public async Task<UserAssignment?> CreateUserAssignmentAsync(CreateUserAssignmentDto dto)
    {
        var existing = await _userAssignmentDAO.GetUserAssignmentAsync(dto.UserId, dto.AssignmentId);

        // Nếu chưa có thì tạo mới (Yêu cầu 4)
        if (existing == null)
        {
            var created = await _userAssignmentDAO.CreateUserAssignmentAsync(dto);

            // ➕ Tạo thêm UserLesson nếu chưa có
            var assignment = await _assignmentDAO.GetByIdAsync(dto.AssignmentId);
            if (assignment != null && assignment.LessonId != 0)
            {
                var userLesson = await _userlessonDAO.GetUserLessonbyuserandlessonAsync(dto.UserId, assignment.LessonId);
                if (userLesson == null)
                {
                    var newUserLesson = new UserLesson
                    {
                        UserId = dto.UserId,
                        LessonId = assignment.LessonId,
                        IsPassed = false,
                        // thêm các field khác nếu có: StartDate, Progress, v.v.
                    };
                    await _userlessonDAO.AddUserLessonAsync(newUserLesson);
                }
            }

            return created;
        }

        // ✅ Lấy assignment tách biệt thay vì dùng existing.Assignment
        var assignmentDetail = await _assignmentDAO.GetByIdAsync(dto.AssignmentId);
        if (assignmentDetail == null || assignmentDetail.LessonId == 0)
            return null;

        var existingUserLesson = await _userlessonDAO.GetUserLessonbyuserandlessonAsync(dto.UserId, assignmentDetail.LessonId);

        if (existingUserLesson != null && existingUserLesson.IsPassed == false)
        {
            // TH1: IsPresented = true && IsScored = true → Update lại
            if (existing.IsPresented && existing.IsScored)
            {
                await _userAssignmentDAO.UpdateUserAssignmentAsync(existing);
                return existing;
            }

            // TH2 & TH3: Không được tạo lại
            return null;
        }

        // TH4: Đã pass rồi → không tạo lại
        return null;
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

        public async Task<List<UserAssignmentDto>> GetUserAssignmentsByAssignmentIdAsync(int assignmentId)
        {
            var submissions = await _userAssignmentDAO.ListUserAssignmentNotScoresAsync(assignmentId);
            return submissions.Select(ua => new UserAssignmentDto
            {
                UserId = ua.UserId,
                AssignmentId = ua.AssignmentId,
                IsScored = ua.IsScored,
                Content = ua.Content,
                IsPresented = ua.IsPresented,
                FirstName = ua.User?.UserMetaData?.FirstName,
                LastName = ua.User?.UserMetaData?.LastName
            }).ToList();
        }

        public async Task<UserAssignmentDto?> GetUserAssignmentAsync(Guid userId, int assignmentId)
        {
            var assignment = await _userAssignmentDAO.GetUserAssignmentAsync(userId, assignmentId);
            if (assignment == null)
                return null;

            return _mapper.Map<UserAssignmentDto>(assignment);
        }


        public async Task<bool> UpdateUserAssignmentAsync(submitAssignmentDto dto)
        {
            var assignment = await _userAssignmentDAO.GetUserAssignmentAsync(dto.UserId, dto.AssignmentId);
            if (assignment == null)
                return false;

            assignment.Content = dto.Content;
             await _userAssignmentDAO.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkUsersAsPresentAsync(MarkPresentDto dto)
        {
            var assignments = await _userAssignmentDAO.GetUserAssignmentsByUserIdsAndAssignmentIdAsync(dto.UserIds, dto.AssignmentId);

            if (assignments == null || assignments.Count == 0)
                return false;

            foreach (var assignment in assignments)
            {
                assignment.IsPresented = true;
            }

             await _userAssignmentDAO.SaveChangesAsync();
            return true;
        }

        public async Task<bool> GradeSubmissionAsync(GradeAssignmentDto grade)
        {
            // Lấy userLesson
            var userLesson = await _userlessonDAO.GetUserLessonbyuserandlessonAsync(grade.UserId, grade.LessonId);

            // Nếu không có thì tạo mới
            if (userLesson == null)
            {
                userLesson = new UserLesson
                {
                    UserId = grade.UserId,
                    LessonId = grade.LessonId
                };
                await _userlessonDAO.AddUserLessonAsync(userLesson);
            }

            // Lấy userAssignment
            var userAssignment = await _userAssignmentDAO.GetUserAssignmentAsync(grade.UserId, grade.AssignmentId);
            if (userAssignment == null)
                return false; // Không tìm thấy assignment => không thể chấm điểm

            // Gán giá trị chấm điểm
            userLesson.Mark = grade.Mark;
            userLesson.CompletedAt = DateTime.UtcNow;
            userLesson.IsPassed = grade.Mark >= 80; // tuỳ điều kiện
            userAssignment.IsScored = true;

            // Lưu thay đổi
            await _lessonDAO.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserAssignmentInfoDto>> ListAssignmentsNotPresentAsync(int assignmentId)
        {
            return await _userAssignmentDAO.ListAssignmentsNotPresentAsync(assignmentId);
        }
    }
}
