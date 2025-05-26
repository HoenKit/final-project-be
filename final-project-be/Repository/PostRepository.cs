using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.Comment;
using final_project_be.Dtos.Post;
using final_project_be.Dtos.User;
using final_project_be.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace final_project_be.Repository
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly PostDAO _postDAO;
        private readonly PostFileDAO _postFileDAO;
        private readonly CommentDAO _commentDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<PostRepository> _logger;
        public PostRepository(PostDAO postDAO,PostFileDAO postFileDAO, CommentDAO commentDAO, IMapper mapper, ILogger<PostRepository> logger) : base(postDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _postDAO = postDAO;
            _postFileDAO = postFileDAO;
            _commentDAO = commentDAO;
        }
        //Update Creat Post
        public async Task<Post> CreatePost(PostCreateDto dto)
        {
            try
            {
                _postDAO.BeginTransaction();
                if (dto.ParentPostId == 0)
                {
                    dto.ParentPostId = null;
                }
                var Post = _mapper.Map<Post>(dto);
                Post.PostFiles = null;
                _postDAO.Add(Post);

                if (dto.PostFileCreate != null && dto.PostFileCreate.Count > 0)
                {
                    Post.PostFiles = dto.PostFileCreate.Select(f => new PostFile
                    {
                        FileUrl = f.FileUrl,
                        PostFileType = f.PostFileType,
                        IsDeleted = f.IsDeleted ?? false,
                        PostId = Post.PostId
                    }).ToList();

                    foreach (var file in Post.PostFiles)
                    {
                        _postFileDAO.Add(file);
                    }
                }

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

        //Update Delete Post
        public bool DeletePost(int id)
        {
            try
            {
                _postDAO.BeginTransaction();

                var post = _postDAO.GetPostWithFilesAndComments(id);
                if (post == null)
                {
                    _logger.LogWarning("Post does not exist.");
                    _postDAO.RollbackTransaction();
                    return false;
                }

                if (post.PostFiles?.Any() == true)
                {
                    foreach (var file in post.PostFiles.ToList())
                    {
                        _postFileDAO.Delete(file.PostFileId);
                    }
                }

                if (post.Comments?.Any() == true)
                {
                    foreach (var comment in post.Comments.ToList())
                    {
                        _commentDAO.Delete(comment.CommentId);
                    }
                }

                _postDAO.Delete(id);
                _postDAO.CommitTransaction();

                _logger.LogInformation("Delete Post success.");
                return true;
            }
            catch (Exception ex)
            {
                _postDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when deleting Post.");
                return false;
            }
        }

        //Update Get All Posts
        public PageResult<PostDto> GetAllPosts(int page, int pageSize, int? CategoryId, string? title, Guid? userId)
        {
            try
            {
                var baseQuery = _postDAO.GetAll()
                    .Include(p => p.User)
                        .ThenInclude(u => u.UserMetaData)
                    .Include(p => p.PostFiles)
                    .Include(p => p.Comments)
                    .OrderByDescending(p => p.CreateAt);

                var query = baseQuery.Where(p => p.IsDeleted == false);

                if (CategoryId != null)
                {
                    query = query.Where(c => c.CategoryId == CategoryId);
                }

                if (!string.IsNullOrEmpty(title))
                {
                    query = query.Where(c => c.Title.Contains(title));
                }

                if (userId.HasValue && userId != Guid.Empty)
                {
                    query = query.Where(p => p.UserId == userId.Value);
                }

                var totalCount = query.Count();

                var posts = query
                    .OrderByDescending(p => p.CreateAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var postDtos = posts.Select(p => new PostDto
                {
                    PostId = p.PostId,
                    Title = p.Title,
                    UserId = p.UserId,
                    Content = p.Content,
                    CategoryId = p.CategoryId,
                    CreateAt = p.CreateAt,
                    PostFiles = p.PostFiles.Select(f => new PostFileDto
                    {
                        PostFileId = f.PostFileId,
                        FileUrl = f.FileUrl,
                        IsDeleted = f.IsDeleted,
                        PostFileType = f.PostFileType
                    }).ToList(),
                    Comments = p.Comments.Select(c => new CommentDto
                    {
                        CommentId = c.CommentId,
                        Content = c.Content,
                        UserId = c.UserId,
                        ParentCommentId = c.ParentCommentId,
                    }).ToList()
                }).ToList();

                _logger.LogInformation("Get Posts success");

                return new PageResult<PostDto>(postDtos, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting Posts");
                return new PageResult<PostDto>(new List<PostDto>(), 0, page, pageSize);
            }
        }

        public async Task<Post> GetPostandUser(int id)
        {
            try
            {
                _postDAO.BeginTransaction();
                var Post = _postDAO.GetPostandUser(id);
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

        //Update UpdatePost
        public async Task<Post> UpdatePost(PostCreateDto dto)
        {
            try
            {
                _postDAO.BeginTransaction();
                if (dto.ParentPostId == 0)
                {
                    dto.ParentPostId = null;
                }
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

        public async Task<Post> ToggleIsDeleted(int id)
        {
            _postDAO.BeginTransaction();
            try
            {
                var post = _postDAO.GetById(id);
                if (post == null)
                {
                    _logger.LogWarning($"Post with ID {id} not found.");
                    return null;
                }

                post.IsDeleted = !post.IsDeleted;
                post.UpdateAt = DateTime.Now;

                _postDAO.Update(post);
                _postDAO.CommitTransaction();

                _logger.LogInformation($"Post {id} banned status changed to {post.IsDeleted}");

                return post;
            }
            catch (Exception ex)
            {
                _postDAO.RollbackTransaction();
                _logger.LogError($"Failed to toggle deleted status for Post {id}: {ex.Message}");
                return null;
            }
        }

        public List<MonthlyStatDto> GetPostStatisticsByMonth()
        {
            var allPosts = _postDAO.GetAll()
                .Where(u => u.CreateAt != null)
                .ToList();

            var stats = allPosts
                .GroupBy(u => new { u.CreateAt.Year, u.CreateAt.Month })
                .Select(g => new MonthlyStatDto
                {
                    Month = $"{g.Key.Month:D2}/{g.Key.Year}",
                    Total = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

            return stats;
        }

        public PageResult<PostDto> GetAllPostsIsDeleted(int page, int pageSize, int? CategoryId, string? title, Guid? userId)
        {
            try
            {
                var baseQuery = _postDAO.GetAll()
                    .Include(p => p.User)
                        .ThenInclude(u => u.UserMetaData)
                    .Include(p => p.PostFiles);

                var query = baseQuery.Where(p => p.IsDeleted == true);

                query = query.OrderByDescending(p => p.UpdateAt);

                if (CategoryId != null)
                {
                    query = query.Where(c => c.CategoryId == CategoryId);
                }

                if (!string.IsNullOrEmpty(title))
                {
                    query = query.Where(c => c.Title.Contains(title));
                }

                if (userId.HasValue && userId != Guid.Empty)
                {
                    query = query.Where(p => p.UserId == userId.Value);
                }

                var totalCount = query.Count();

                var posts = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var postDtos = posts.Select(p => new PostDto
                {
                    PostId = p.PostId,
                    Title = p.Title,
                    UserId = p.UserId,
                    Content = p.Content,
                    CategoryId = p.CategoryId,
                    CreateAt = p.CreateAt,
                    PostFiles = p.PostFiles.Select(f => new PostFileDto
                    {
                        PostFileId = f.PostFileId,
                        FileUrl = f.FileUrl,
                        IsDeleted = f.IsDeleted,
                        PostFileType = f.PostFileType
                    }).ToList(),
                }).ToList();

                _logger.LogInformation("Get Deleted Posts success");

                return new PageResult<PostDto>(postDtos, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting Deleted Posts");
                return new PageResult<PostDto>(new List<PostDto>(), 0, page, pageSize);
            }
        }
    }
}
