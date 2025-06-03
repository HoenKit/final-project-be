using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Post;
using final_project_be_Domain.DTOs.Users;

namespace final_project_be_Application.Interface
{
    public interface IPostRepository : IRepository<Post>
    {
        public Task<Post> CreatePost(PostCreateDto dto);
        public Task<bool> DeletePost(int id);
        public Task<Post> GetPost(int id);
        public Task<Post> UpdatePost(PostCreateDto dto);
        public Task<Post> GetPostandUser(int id);
        public PageResult<PostDto> GetAllPosts(int page, int pageSize, int? subCategoryId, string? title, Guid? userId); //UpdateAsync GetAllPosts
        public Task<Post> ToggleIsDeleted(int id);
        public List<MonthlyStatDto> GetPostStatisticsByMonth();
        public PageResult<PostDto> GetAllPostsIsDeleted(int page, int pageSize, int? CategoryId, string? title, Guid? userId);
    }
}
