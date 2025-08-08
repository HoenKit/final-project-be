using AutoMapper;
using final_project_be_Infrastructure.DAO;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.DTOs.Post;
using final_project_be_Domain.DTOs.Users;
using final_project_be_Application.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using final_project_be_Infrastructure.DAO_Interface;

namespace final_project_be_Application.Repository
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly IPostDAO _postDAO;
        private readonly IPostFileDAO _postFileDAO;
        private readonly ICommentDAO _commentDAO;
        private readonly IBlobStorageService _blobStorageService;
        private readonly IMapper _mapper;
        private readonly ILogger<PostRepository> _logger;
        public PostRepository(IPostDAO postDAO, IPostFileDAO postFileDAO, ICommentDAO commentDAO, IMapper mapper, ILogger<PostRepository> logger, IBlobStorageService blobStorageService) : base(postDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _postDAO = postDAO;
            _postFileDAO = postFileDAO;
            _commentDAO = commentDAO;
            _blobStorageService = blobStorageService;
        }

        public async Task<Post> CreatePost(PostCreateDto dto)
        {
            try
            {
                await _postDAO.BeginTransactionAsync();

                if (dto.ParentPostId == 0)
                {
                    dto.ParentPostId = null;
                }

                var post = _mapper.Map<Post>(dto);
                post.PostFiles = null;

                await _postDAO.AddAsync(post);

                if (dto.PostFileLinks != null && dto.PostFileLinks.Count > 0)
                {
                    post.PostFiles = new List<PostFile>();

                    foreach (var formFile in dto.PostFileLinks)
                    {
                        if (formFile.Length > 0)
                        {
                            var fileExtension = Path.GetExtension(formFile.FileName);
                            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                            using (var stream = formFile.OpenReadStream())
                            {
                                await _blobStorageService.UploadFileAsync(uniqueFileName, stream);
                            }

                            string postFileType = fileExtension.ToLower() switch
                            {
                                ".jpg" or ".jpeg" or ".png" or ".gif" => "Image",
                                ".mp4" or ".mov" or ".avi" or ".mkv" => "Video",
                                _ => "Unknown"
                            };

                            var fileUrl = $"https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/{uniqueFileName}";

                            var postFile = new PostFile
                            {
                                FileUrl = fileUrl,
                                PostFileType = postFileType,
                                IsDeleted = false,
                                PostId = post.PostId
                            };

                            await _postFileDAO.AddAsync(postFile);
                        }
                    }
                }

                await _postDAO.CommitTransactionAsync();

                _logger.LogInformation("AddAsync Post success");
                return post;
            }
            catch (Exception ex)
            {
                await _postDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding Post");
                return null;
            }
        }

        public async Task<bool> DeletePost(int id)
        {
            try
            {
                await _postDAO.BeginTransactionAsync();

                var post = _postDAO.GetPostWithFilesAndComments(id);
                if (post == null)
                {
                    _logger.LogWarning("Post does not exist.");
                    await _postDAO.RollbackTransactionAsync();
                    return false;
                }

                if (post.PostFiles?.Any() == true)
                {
                    foreach (var file in post.PostFiles.ToList())
                    {
                        await _postFileDAO.DeleteAsync(file.PostFileId);
                    }
                }

                if (post.Comments?.Any() == true)
                {
                    foreach (var comment in post.Comments.ToList())
                    {
                        await _commentDAO.DeleteAsync(comment.CommentId);
                    }
                }

                await _postDAO.DeleteAsync(id);
                await _postDAO.CommitTransactionAsync();

                _logger.LogInformation("DeleteAsync Post success.");
                return true;
            }
            catch (Exception ex)
            {
                await _postDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when deleting Post.");
                return false;
            }
        }

        public PageResult<PostDto> GetAllPosts(int page, int pageSize, int? CategoryId, string? title, Guid? userId, bool? IsDeleted)
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

                if (IsDeleted == true)
                {
                    query = baseQuery.Where(p => p.IsDeleted == true);
                }

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
                await _postDAO.BeginTransactionAsync();
                var Post = _postDAO.GetPostandUser(id);
                await _postDAO.CommitTransactionAsync();

                _logger.LogInformation("Get Post success");
                return Post;
            }
            catch (Exception ex)
            {
                await _postDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when get Post");
                return null;
            }
        }
        public async Task<Post> GetPost(int id)
        {
            try
            {
                await _postDAO.BeginTransactionAsync();
                var Post = await _postDAO.GetByIdAsync(id);
                await _postDAO.CommitTransactionAsync();

                _logger.LogInformation("Get Post success");
                return Post;
            }
            catch (Exception ex)
            {
                await _postDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when get Post");
                return null;
            }

        }

        public async Task<Post> UpdatePost(PostCreateDto dto)
        {
            try
            {
                await _postDAO.BeginTransactionAsync();

                if (dto.ParentPostId == 0)
                {
                    dto.ParentPostId = null;
                }

                var existingPost = await _postDAO.GetByIdAsync(dto.PostId);
                if (existingPost == null)
                {
                    throw new Exception("Post not found.");
                }

                existingPost.Content = dto.Content;
                existingPost.Title = dto.Title;
                existingPost.ParentPostId = dto.ParentPostId;
                existingPost.CategoryId = dto.CategoryId;
                existingPost.UpdateAt = DateTime.Now;

                await _postDAO.UpdateAsync(existingPost);


                var existingFiles = await _postFileDAO.GetByPostIdAsync(dto.PostId);
                foreach (var file in existingFiles)
                {
                    var fileName = Path.GetFileName(new Uri(file.FileUrl).LocalPath);
                    await _blobStorageService.DeleteFileIfExistsAsync(fileName);
                    await _postFileDAO.DeleteAsync(file.PostFileId);
                }

                if (dto.PostFileLinks != null && dto.PostFileLinks.Count > 0)
                {
                    foreach (var formFile in dto.PostFileLinks)
                    {
                        if (formFile.Length > 0)
                        {
                            var fileExtension = Path.GetExtension(formFile.FileName);
                            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                            using (var stream = formFile.OpenReadStream())
                            {
                                await _blobStorageService.UploadFileAsync(uniqueFileName, stream);
                            }

                            string postFileType = fileExtension.ToLower() switch
                            {
                                ".jpg" or ".jpeg" or ".png" or ".gif" => "Image",
                                ".mp4" or ".mov" or ".avi" or ".mkv" => "Video",
                                _ => "Unknown"
                            };

                            var fileUrl = $"https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/{uniqueFileName}";

                            var postFile = new PostFile
                            {
                                FileUrl = fileUrl,
                                PostFileType = postFileType,
                                IsDeleted = false,
                                PostId = existingPost.PostId
                            };

                            await _postFileDAO.AddAsync(postFile);
                        }
                    }
                }

                await _postDAO.CommitTransactionAsync();

                _logger.LogInformation("UpdateAsync Post success");
                return existingPost;
            }
            catch (Exception ex)
            {
                await _postDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when UpdateAsync Post");
                return null;
            }
        }

        public async Task<Post> ToggleIsDeleted(int id)
        {
            await _postDAO.BeginTransactionAsync();
            try
            {
                var post = await _postDAO.GetByIdAsync(id);
                if (post == null)
                {
                    _logger.LogWarning($"Post with ID {id} not found.");
                    return null;
                }

                post.IsDeleted = !post.IsDeleted;
                post.UpdateAt = DateTime.Now;

                await _postDAO.UpdateAsync(post);
                await _postDAO.CommitTransactionAsync();

                _logger.LogInformation($"Post {id} banned status changed to {post.IsDeleted}");

                return post;
            }
            catch (Exception ex)
            {
                await _postDAO.RollbackTransactionAsync();
                _logger.LogError($"Failed to toggle deleted status for Post {id}: {ex.Message}");
                return null;
            }
        }

        public List<MonthlyStatDto> GetPostStatisticsByMonth()
        {
            try
            {
                var allPosts = _postDAO.GetAll()
                    .Where(u => u.CreateAt != null)
                    .ToList();

                _logger.LogInformation($"Processing {allPosts.Count} posts for monthly statistics.");

                var stats = allPosts
                    .GroupBy(u => new { u.CreateAt.Year, u.CreateAt.Month })
                    .Select(g => new MonthlyStatDto
                    {
                        Month = $"{g.Key.Month:D2}/{g.Key.Year}",
                        Total = g.Count()
                    })
                    .OrderBy(x => x.Month)
                    .ToList();

                _logger.LogInformation("Successfully generated post statistics by month.");
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to calculate post statistics by month.");
                return new List<MonthlyStatDto>();
            }
        }

        public async Task<PostDetailDto> GetPostDetail(int id)
        {
            try
            {
                await _postDAO.BeginTransactionAsync();

                var post = _postDAO.GetAll()
                    .Include(p => p.User)
                        .ThenInclude(u => u.UserMetaData)
                    .Include(p => p.PostFiles)
                    .Include(p => p.Comments)
                        .ThenInclude(c => c.User)
                        .ThenInclude(u => u.UserMetaData)
                    .Include(p => p.Category)
                    .FirstOrDefault(p => p.PostId == id);

                if (post == null)
                {
                    return null;
                }

                var postDetailDto = new PostDetailDto
                {
                    PostId = post.PostId,
                    UserId = post.UserId,
                    FirstName = post.User?.UserMetaData.FirstName,
                    LastName = post.User?.UserMetaData.LastName,
                    Avatar = post.User?.UserMetaData.Avatar,
                    ParentPostId = post.ParentPostId,
                    CategoryName = post.Category.Title,
                    IsDeleted = post.IsDeleted,
                    Title = post.Title,
                    Content = post.Content,
                    CreateAt = post.CreateAt,
                    UpdateAt = post.UpdateAt,
                    PostFiles = post.PostFiles?.Select(pf => new PostFileDto
                    {
                        PostFileId = pf.PostFileId,
                        PostId = pf.PostId,
                        FileUrl = pf.FileUrl,
                        PostFileType = pf.PostFileType,
                        IsDeleted = pf.IsDeleted
                    }).ToList(),
                    Comments = post.Comments?.Select(c => new CommentPostDetailDto
                    {
                        CommentId = c.CommentId,
                        PostId = c.PostId,
                        UserId = c.UserId,
                        FirstName = c.User?.UserMetaData?.FirstName,
                        LastName = c.User?.UserMetaData?.LastName,
                        Avatar = c.User?.UserMetaData?.Avatar,
                        ParentCommentId = c.ParentCommentId,
                        Content = c.Content
                    }).ToList()
                };

                await _postDAO.CommitTransactionAsync();
                _logger.LogInformation("Get Post success");
                return postDetailDto;
            }
            catch (Exception ex)
            {
                await _postDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when get Post");
                return null;
            }
        }
    }
}
