using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.Post;

namespace final_project_be.Interface
{
    public interface IPostRepository : IRepository<Post>
    {
        public Task<Post> CreatePost(PostCreateDto dto);
        public bool DeletePost(int id);
        public Task<Post> GetPost(int id);
        public Task<Post> UpdatePost(PostCreateDto dto);
        public Task<Post> GetPostandUser(int id);
        public PageResult<PostDto> GetAllPosts(int page, int pageSize, int? subCategoryId, string? title, Guid? userId); //Update GetAllPosts
        public Task<Post> ToggleIsDeleted(int id);
    }
}
