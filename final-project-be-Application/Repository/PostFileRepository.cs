using AutoMapper;
using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Post;
using Microsoft.Extensions.Logging;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Application.Repository
{
	public class PostFileRepository : Repository<PostFile>, IPostFileRepository
	{
		private readonly IPostFileDAO _postFileDAO;
		private readonly IMapper _mapper;
		private readonly ILogger<PostFileRepository> _logger;

		public PostFileRepository(IPostFileDAO postFileDAO, IMapper mapper, ILogger<PostFileRepository> logger) : base(postFileDAO)
		{
			_postFileDAO = postFileDAO;
			_mapper = mapper;
			_logger = logger;
		}

		public IEnumerable<PostFile> GetAllPostFilesByPostId(int postId)
		{
			try
			{
				var postFiles = _postFileDAO.GetAll()
					.Where(p => p.PostId == postId)
					.ToList();

				_logger.LogInformation("Get postfiles success");

				return postFiles;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error when getting postfiles");

				return Enumerable.Empty<PostFile>();
			}
		}
		public async Task<PostFile> CreatePostFile(PostFileDto dto)
		{
			try
			{
				await _postFileDAO.BeginTransactionAsync();
				var PostFile = _mapper.Map<PostFile>(dto);
				await _postFileDAO.AddAsync(PostFile);
				await _postFileDAO.CommitTransactionAsync();

				_logger.LogInformation("AddAsync PostFile success");
				return PostFile;
			}
			catch (Exception ex)
			{
				await _postFileDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when adding PostFile");
				return null;
			}
		}



		public async Task<bool> DeletePostFile(int id)
		{
			try
			{
				await _postFileDAO.BeginTransactionAsync();
				await _postFileDAO.DeleteAsync(id);
				await _postFileDAO.CommitTransactionAsync();

				_logger.LogInformation("DeleteAsync PostFile success");
				return true;
			}
			catch (Exception ex)
			{
				await _postFileDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when delete PostFile");
				return false;
			}
		}

		public async Task<PostFile> GetPostFile(int id)
		{
			try
			{
				await _postFileDAO.BeginTransactionAsync();
				var postFile = await _postFileDAO.GetByIdAsync(id);
				await _postFileDAO.CommitTransactionAsync();

				_logger.LogInformation("Get postFile success");
				return postFile;
			}
			catch (Exception ex)
			{
				await _postFileDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when get postFile");
				return null;
			}
		}
	}
}
