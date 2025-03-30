using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Interface;
using final_project_be.Dtos.Post;

namespace final_project_be.Repository
{
	public class PostFileRepository : Repository<PostFile>, IPostFileRepository
	{
		private readonly PostFileDAO _postFileDAO;
		private readonly IMapper _mapper;
		private readonly ILogger<PostFileRepository> _logger;

		public PostFileRepository(PostFileDAO postFileDAO, IMapper mapper, ILogger<PostFileRepository> logger) : base(postFileDAO)
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
				_postFileDAO.BeginTransaction();
				var PostFile = _mapper.Map<PostFile>(dto);
				_postFileDAO.Add(PostFile);
				_postFileDAO.CommitTransaction();

				_logger.LogInformation("Add PostFile success");
				return PostFile;
			}
			catch (Exception ex)
			{
				_postFileDAO.RollbackTransaction();
				_logger.LogError(ex, "Error when adding PostFile");
				return null;
			}
		}

		public bool DeletePostFile(int id)
		{
			try
			{
				_postFileDAO.BeginTransaction();
				_postFileDAO.Delete(id);
				_postFileDAO.CommitTransaction();

				_logger.LogInformation("Delete PostFile success");
				return true;
			}
			catch (Exception ex)
			{
				_postFileDAO.RollbackTransaction();
				_logger.LogError(ex, "Error when delete PostFile");
				return false;
			}
		}

		public async Task<PostFile> GetPostFile(int id)
		{
			try
			{
				_postFileDAO.BeginTransaction();
				var postFile = _postFileDAO.GetById(id);
				_postFileDAO.CommitTransaction();

				_logger.LogInformation("Get postFile success");
				return postFile;
			}
			catch (Exception ex)
			{
				_postFileDAO.RollbackTransaction();
				_logger.LogError(ex, "Error when get postFile");
				return null;
			}
		}
	}
}
