using AutoMapper;
using final_project_be.Data.Models;
using final_project_be.Dtos.Comment;
using final_project_be.Dtos.Notification;
using final_project_be.Dtos.PollOption;
using final_project_be.Dtos.Report;
using final_project_be.Dtos.User;

namespace final_project_be.Service.Mapping
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
            CreateMap<UserManagerDto, User>().ReverseMap();
        }
    }
}
