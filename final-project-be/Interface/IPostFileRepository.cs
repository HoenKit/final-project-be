using final_project_be.Data.Models;
using final_project_be.Dtos.Post;


namespace final_project_be.Interface
{
	public interface IPostFileRepository : IRepository<PostFile>
	{
		public IEnumerable<PostFile> GetAllPostFilesByPostId(int postId);
		public Task<PostFile> CreatePostFile(PostFileDto dto);
		public bool DeletePostFile(int id);
		public Task<PostFile> GetPostFile(int id);
	}
}
