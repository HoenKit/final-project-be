using AutoMapper;
using final_project_be.Data.Models;
using final_project_be.Dtos.Category;
using final_project_be.Dtos.Comment;
using final_project_be.Dtos.PollOption;

namespace final_project_be.Service.Mapping
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CommentDto, Comment>().ReverseMap();
            CreateMap<CategoryDto, Category>().ReverseMap();
            CreateMap<SubCategoryDto, SubCategory>().ReverseMap();
            CreateMap<PollOptionDto, PollOption>().ReverseMap();
            CreateMap<PollOptionVoteDto, PollOptionVote>().ReverseMap();
        }
    }
}
