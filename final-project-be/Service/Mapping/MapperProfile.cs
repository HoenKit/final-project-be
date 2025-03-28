using AutoMapper;
using final_project_be.Data.Models;
using final_project_be.Dtos.Comment;
using final_project_be.Dtos.Post;

namespace final_project_be.Service.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CommentDto, Comment>().ReverseMap();
            CreateMap<PostDto, Post>().ReverseMap();
        }
    }
}
