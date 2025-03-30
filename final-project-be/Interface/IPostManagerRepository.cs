using final_project_be.Data.Models;
using final_project_be.Dtos;

namespace final_project_be.Interface
{
    public interface IPostManagerRepository : IRepository<Post>
    {
        public Task<PageResult<Post>> GetAllPosts(int page, int pageSize, bool prioritizeReports = false);
        public Task<PageResult<Post>> GetPostsByUser(Guid userId, int page, int pageSize, bool prioritizeReports = false);
        public Task<PageResult<Post>> GetPostsBySubCategory(int subCategoryId, int page, int pageSize, bool prioritizeReports = false);
        public Task<Post> ToggleIsDeleted(int postId);
        public Task<Post> GetPost(int postId);
    }
}
