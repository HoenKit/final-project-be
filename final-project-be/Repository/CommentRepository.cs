using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data;
using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.Comment;
using final_project_be.Interface;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace final_project_be.Repository
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly CommentDAO _commentDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<CommentRepository> _logger;

        public CommentRepository(CommentDAO commentDAO, IMapper mapper, ILogger<CommentRepository> logger) : base(commentDAO)
        {
            _mapper = mapper;
            _logger = logger;
            _commentDAO = commentDAO;
        }

        public async Task<Comment> CreateComment(CommentDto dto)
        {
            try
            {
                _commentDAO.BeginTransaction();
                var comment = _mapper.Map<Comment>(dto);
                _commentDAO.Add(comment);
                _commentDAO.CommitTransaction();

                _logger.LogInformation("Add comment success");
                return comment;
            }
            catch (Exception ex)
            {
                _commentDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when adding comment");
                return null;
            }
        }

        public bool DeleteComment(int id)
        {
            try
            {
                _commentDAO.BeginTransaction();
                _commentDAO.Delete(id);
                _commentDAO.CommitTransaction();

                _logger.LogInformation("Delete comment success");
                return true;
            }
            catch (Exception ex)
            {
                _commentDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when delete comment");
                return false;
            }
        }

        public PageResult<Comment> GetAllCommentsByPostId(int page, int pageSize, int postId)
        {
            try
            {
                var totalCount = _commentDAO.GetAll().Count(); 
                var comments = _commentDAO.GetAll()
                    .Where(p => p.PostId == postId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                _logger.LogInformation("Get comments success");

                return new PageResult<Comment>(comments, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting comments");
                return new PageResult<Comment>(new List<Comment>(), 0, page, pageSize); 
            }
        }

        public async Task<Comment> GetComment(int id)
        {
            try
            {
                _commentDAO.BeginTransaction();
                var comment = _commentDAO.GetById(id);
                _commentDAO.CommitTransaction();

                _logger.LogInformation("Get comment success");
                return comment;
            }
            catch (Exception ex)
            {
                _commentDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when get comment");
                return null;
            }

        }

        public async Task<Comment> UpdateComment(CommentDto dto)
        {
            try
            {
                _commentDAO.BeginTransaction();
                var comment = _mapper.Map<Comment>(dto);
                _commentDAO.Update(comment);
                _commentDAO.CommitTransaction();

                _logger.LogInformation("Update comment success");
                return comment;
            }
            catch (Exception ex)
            {
                _commentDAO.RollbackTransaction();
                _logger.LogError(ex, "Error when update comment");
                return null;
            }
        }
    }
}
