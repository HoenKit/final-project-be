using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.Post;

namespace final_project_be.Interface
{
    public interface IPostRepository : IRepository<Post>
    {
        public Task<Post> CreatePost(PostDto dto);
        public bool DeletePost(int id);
        public Task<Post> GetPost(int id);
        public Task<Post> UpdatePost(PostDto dto);
        public PageResult<Post> GetAllPosts(int page, int pageSize);

        //Update SearchPosts
        IEnumerable<Post> SearchPosts(string query);

    }
}
