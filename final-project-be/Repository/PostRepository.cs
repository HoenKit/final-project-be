using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.Post;
using final_project_be.Dtos.Post;
using final_project_be.Interface;

namespace final_project_be.Repository
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly PostDAO _postDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<PostRepository> _logger;
        public PostRepository(PostDAO postDAO, IMapper mapper, ILogger<PostRepository> logger) : base(postDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _postDAO = postDAO;

        }

        public async Task<Post> CreatePost(PostDto dto)
        {
            try
            {
                _postDAO.BeginTransaction();
                var Post = _mapper.Map<Post>(dto);
                _postDAO.Add(Post);
                _postDAO.CommitTransaction();

                _logger.LogInformation("Add Post success");
                return Post;
            }
            catch (Exception ex)
            {
                _postDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when adding Post");
                return null;
            }
        }

        public bool DeletePost(int id)
        {
            try
            {
                _postDAO.BeginTransaction();
                _postDAO.Delete(id);
                _postDAO.CommitTransaction();

                _logger.LogInformation("Delete Post success");
                return true;
            }
            catch (Exception ex)
            {
                _postDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when delete Post");
                return false;
            }
        }

        public PageResult<Post> GetAllPosts(int page, int pageSize)
        {
            try
            {
                var totalCount = _postDAO.GetAll().Count();
                var Posts = _postDAO.GetAll()
                    .Where(p => p.IsDeleted == false)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get Posts success");

                return new PageResult<Post>(Posts, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting Posts");
                return new PageResult<Post>(new List<Post>(), 0, page, pageSize);
            }
        }

        public async Task<Post> GetPost(int id)
        {
            try
            {
                _postDAO.BeginTransaction();
                var Post = _postDAO.GetById(id);
                _postDAO.CommitTransaction();

                _logger.LogInformation("Get Post success");
                return Post;
            }
            catch (Exception ex)
            {
                _postDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get Post");
                return null;
            }

        }

        public async Task<Post> UpdatePost(PostDto dto)
        {
            try
            {
                _postDAO.BeginTransaction();
                var Post = _mapper.Map<Post>(dto);
                _postDAO.Update(Post);
                _postDAO.CommitTransaction();

                _logger.LogInformation("Update Post success");
                return Post;
            }
            catch (Exception ex)
            {
                _postDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update Post");
                return null;
            }
        }
        //Update SearchPosts
        public IEnumerable<Post> SearchPosts(string query)
        {
            return _postDAO.SearchPosts(query);
        }
        //Get post by userid
        public async Task<PageResult<PostDto>> GetPostsByUserId(Guid userId, int page, int pageSize)
        {
            try
            {
                _postDAO.BeginTransaction();

                // Get list of posts by userId
                var query = _postDAO.Find(p => p.UserId == userId);

                int totalItems = query.Count();

                // pagination
                var pagedPosts = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _postDAO.CommitTransaction();
                _logger.LogInformation($"Lấy danh sách bài viết của user {userId} thành công.");

                var postDtos = _mapper.Map<List<PostDto>>(pagedPosts);

                return new PageResult<PostDto>(postDtos, totalItems, page, pageSize);
            }
            catch (Exception ex)
            {
                _postDAO.RollbackTransaction();
                _logger.LogError(ex, $"Lỗi khi lấy danh sách bài viết của user {userId}");

                return new PageResult<PostDto>(new List<PostDto>(), 0, page, pageSize);
            }
        }
    }
}
