using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs.Post;


namespace final_project_be_Application.Interface
{
	public interface IPostFileRepository : IRepository<PostFile>
	{
		public IEnumerable<PostFile> GetAllPostFilesByPostId(int postId);
		public Task<PostFile> CreatePostFile(PostFileDto dto);
		public Task<bool> DeletePostFile(int id);
		public Task<PostFile> GetPostFile(int id);
	}
}
