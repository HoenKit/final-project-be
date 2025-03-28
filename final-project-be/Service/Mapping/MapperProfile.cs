using AutoMapper;
using final_project_be.Data.Models;
using final_project_be.Dtos.Comment;
using final_project_be.Dtos.User;

namespace final_project_be.Service.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CommentDto, Comment>().ReverseMap();
            CreateMap<UserManagerDto, User>().ReverseMap();
        }
    }
}
