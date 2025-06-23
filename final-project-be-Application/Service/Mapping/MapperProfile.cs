using AutoMapper;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.DTOs.Notification;
using final_project_be_Domain.DTOs.PollOption;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.DTOs.Users;
using final_project_be_Domain.DTOs.Post;
using final_project_be_Domain.DTOs.Category;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Module;
using final_project_be_Domain.DTOs.Lesson;
using final_project_be_Domain.DTOs.Question;
using final_project_be_Domain.DTOs.Answer;
using final_project_be_Domain.DTOs.Assignment;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Review;

namespace final_project_be_Application.Service.Mapping
{
	public class MapperProfile : Profile
	{
		public MapperProfile()
		{
			CreateMap<CommentDto, Comment>().ReverseMap();
			CreateMap<NotificationDto, Notification>().ReverseMap();
			CreateMap<PollOptionDto, PollOption>().ReverseMap();
			CreateMap<ReportCommentDto, ReportComment>().ReverseMap();
			CreateMap<ReportCommentDto, Report>().ReverseMap();
			CreateMap<ReportCommentDto, ReportDto>().ReverseMap();
			CreateMap<ReportPostDto, ReportPost>().ReverseMap();
			CreateMap<ReportPostDto, Report>().ReverseMap();
			CreateMap<ReportPostDto, ReportDto>().ReverseMap();
			CreateMap<UserManagerDto, User>().ReverseMap();
			CreateMap<PostDto, Post>().ReverseMap();
			CreateMap<CategoryDto, Category>().ReverseMap();
			CreateMap<PollOptionVoteDto, PollOptionVote>().ReverseMap();
			CreateMap<UserRegisterDto, User>().ReverseMap();
			CreateMap<UserLoginDto, User>().ReverseMap();
			CreateMap<ReportUserDto, ReportUser>().ReverseMap();
			CreateMap<ReportUserDto, Report>().ReverseMap();
			CreateMap<ReportUserDto, ReportDto>().ReverseMap();
			CreateMap<UserProfileDto, User>().ReverseMap();
			CreateMap<UserProfileDto, UserMetadata>().ReverseMap();
			CreateMap<PostFileDto, PostFile>().ReverseMap();
			CreateMap<PostCreateDto, Post>().ReverseMap();
			CreateMap<PostFileCreateDto, Post>().ReverseMap();
			CreateMap<Courses, CourseDto>().ReverseMap();
			CreateMap<Courses, UpdateCourseDto>().ReverseMap();
			CreateMap<Courses, CourseResponseDto>().ReverseMap();
			CreateMap<Module, ModuleDto>().ReverseMap();
			CreateMap<Module, UpdateModuleDto>().ReverseMap();
			CreateMap<Module, ModuleResponseDto>().ReverseMap();
			CreateMap<Lesson, UpdateLessonDto>().ReverseMap();
			CreateMap<Lesson, LessonDto>().ReverseMap();
			CreateMap<Lesson, LessonResponseDto>().ReverseMap();
			CreateMap<Question, QuestionDto>().ReverseMap();
			CreateMap<Question, UpdateQuestionDto>().ReverseMap();
			CreateMap<Answer, AnswerDto>().ReverseMap();
			CreateMap<Answer, UpdateAnswerDto>().ReverseMap();
			CreateMap<Assignment, AssignmentDto>().ReverseMap();
			CreateMap<Assignment, UpdateAssignmentDto>().ReverseMap();
			CreateMap<Assignment, AssignmentResponseDto>().ReverseMap();
            CreateMap<Mentor, MentorCertificateDto>().ReverseMap();
            CreateMap<MentorCertificate, MentorCertificateDto>();
            CreateMap<Mentor, CreateMentorDto>().ReverseMap();
            CreateMap<Mentor, GetMentorDto>().ReverseMap();
            CreateMap<MentorCertificate, GetMentorCertificateDto>().ReverseMap();
            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<Review, UpdateReviewDto>().ReverseMap();
            CreateMap<Review, ReviewResponseDto>().ReverseMap();
        }
	}
}
