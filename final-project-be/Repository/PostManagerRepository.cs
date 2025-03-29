using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.User;
using final_project_be.Interface;
using Microsoft.EntityFrameworkCore;

namespace final_project_be.Repository
{
    public class PostManagerRepository : Repository<Post>, IPostManagerRepository
    {
        private readonly PostManagerDAO _postManagerDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<PostManagerRepository> _logger;

        public PostManagerRepository(PostManagerDAO postManagerDAO, IMapper mapper, ILogger<PostManagerRepository> logger)
            : base(postManagerDAO)
        {
            _postManagerDAO = postManagerDAO;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PageResult<Post>> GetAllPosts(int page, int pageSize, bool prioritizeReports = false)
        {
            try
            {
                _logger.LogInformation("Fetching all posts with pagination");

                var query = _postManagerDAO.GetAllPosts()
                            .Select(p => new
                            {
                                Post = p,
                                ReportCount = p.ReportPosts.Count()
                            });

                if (prioritizeReports)
                {
                    query = query.OrderByDescending(p => p.ReportCount > 0)
                                 .ThenByDescending(p => p.ReportCount)
                                 .ThenByDescending(p => p.Post.CreateAt);
                }
                else
                {
                    query = query.OrderBy(p => p.Post.CreateAt);
                }

                var totalCount = query.Count();

                var posts = query
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .Select(p => p.Post)
                            .ToList();

                return new PageResult<Post>(posts, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when fetching paginated posts");
                return new PageResult<Post>(new List<Post>(), 0, page, pageSize);
            }
        }

        public async Task<PageResult<Post>> GetPostsByUser(Guid userId, int page, int pageSize, bool prioritizeReports = false)
        {
            try
            {
                _logger.LogInformation($"Fetching posts for User ID: {userId}");

                var posts = _postManagerDAO.GetAllPosts()
                            .Where(p => p.UserId == userId)
                            .Select(p => new
                            {
                                Post = p,
                                ReportCount = p.ReportPosts.Count()
                            });

                if (prioritizeReports)
                {
                    posts = posts.OrderByDescending(p => p.ReportCount > 0)
                                 .ThenByDescending(p => p.ReportCount)
                                 .ThenByDescending(p => p.Post.CreateAt);
                }
                else
                {
                    posts = posts.OrderByDescending(p => p.Post.CreateAt);
                }

                posts = posts.OrderBy(p => p.Post.IsDeleted)
                             .ThenByDescending(p => p.Post.CreateAt);

                var totalCount = await posts.CountAsync();

                var pagedPosts = await posts
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .Select(p => p.Post)
                                    .ToListAsync();

                return new PageResult<Post>(pagedPosts, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when fetching posts by user");
                return new PageResult<Post>(new List<Post>(), 0, page, pageSize);
            }
        }


        public async Task<PageResult<Post>> GetPostsBySubCategory(int subCategoryId, int page, int pageSize, bool prioritizeReports = false)
        {
            try
            {
                _logger.LogInformation($"Fetching posts for SubCategory ID: {subCategoryId}");

                var posts = _postManagerDAO.GetAllPosts()
                            .Where(p => p.SubCategoryId == subCategoryId)
                            .Select(p => new
                            {
                                Post = p,
                                ReportCount = p.ReportPosts.Count()
                            });

                if (prioritizeReports)
                {
                    posts = posts.OrderByDescending(p => p.ReportCount > 0)
                                 .ThenByDescending(p => p.ReportCount)
                                 .ThenByDescending(p => p.Post.CreateAt);
                }
                else
                {
                    posts = posts.OrderByDescending(p => p.Post.CreateAt);
                }

                posts = posts.OrderBy(p => p.Post.IsDeleted)
                             .ThenByDescending(p => p.Post.CreateAt);

                var totalCount = await posts.CountAsync();

                var pagedPosts = await posts
                                    .Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .Select(p => p.Post)
                                    .ToListAsync();

                return new PageResult<Post>(pagedPosts, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when fetching posts by subcategory");
                return new PageResult<Post>(new List<Post>(), 0, page, pageSize);
            }
        }


        public async Task<Post> ToggleIsDeleted(int postId)
        {
            _postManagerDAO.BeginTransaction();
            try
            {
                var post = _postManagerDAO.GetById(postId);
                if (post == null)
                {
                    _logger.LogWarning($"Post with ID {postId} not found.");
                    return null;
                }

                post.IsDeleted = !post.IsDeleted;
                post.UpdateAt = DateTime.Now;

                _postManagerDAO.Update(post);
                _postManagerDAO.CommitTransaction();

                _logger.LogInformation($"Post {postId} banned status changed to {post.IsDeleted}");

                return post;
            }
            catch (Exception ex)
            {
                _postManagerDAO.RollbackTransaction();
                _logger.LogError($"Failed to toggle ban status for Post {postId}: {ex.Message}");
                return null;
            }
        }

        public async Task<Post> GetPost(int postId)
        {
            try
            {
                _postManagerDAO.BeginTransaction();
                var post = _postManagerDAO.GetPostById(postId);
                _postManagerDAO.CommitTransaction();

                _logger.LogInformation("Get post success");
                return post;
            }
            catch (Exception ex)
            {
                _postManagerDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get post");
                return null;
            }

        }
    }
}
