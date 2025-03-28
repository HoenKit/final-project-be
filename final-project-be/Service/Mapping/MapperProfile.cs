using AutoMapper;
using final_project_be.Data.Models;
using final_project_be.Dtos.Category;
using final_project_be.Dtos.Comment;
using final_project_be.Dtos.PollOption;
using final_project_be.Dtos.Notification;
using final_project_be.Dtos.Report;
using final_project_be.Dtos.User;
using final_project_be.Dtos.Post;
using final_project_be.Dtos.SubCategory;


namespace final_project_be.Service.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CommentDto, Comment>().ReverseMap();
            CreateMap<CategoryDto, Category>().ReverseMap();
            CreateMap<SubCategoryDto, SubCategory>().ReverseMap();
            CreateMap<PollOptionVoteDto, PollOptionVote>().ReverseMap();
            CreateMap<NotificationDto, Notification>().ReverseMap();
            CreateMap<PollOptionDto, PollOption>().ReverseMap();
            CreateMap<ReportCommentDto, ReportComment>().ReverseMap();
            CreateMap<ReportCommentDto, Report>().ReverseMap();
            CreateMap<ReportCommentDto, ReportDto>().ReverseMap();
            CreateMap<UserManagerDto, User>().ReverseMap();
            CreateMap<PostDto, Post>().ReverseMap();
        }
    }
}
