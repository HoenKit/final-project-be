using AutoMapper;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.Data;
using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Comment;
using final_project_be_Application.Interface;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Logging;

namespace final_project_be_Application.Repository
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
				await _commentDAO.BeginTransactionAsync();
				var comment = _mapper.Map<Comment>(dto);
				await _commentDAO.AddAsync(comment);
				await _commentDAO.CommitTransactionAsync();

				_logger.LogInformation("AddAsync comment success");
				return comment;
			}
			catch (Exception ex)
			{
				await _commentDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when adding comment");
				return null;
			}
		}

		public async Task<bool> DeleteComment(int id)
		{
			try
			{
				await _commentDAO.BeginTransactionAsync();
				await _commentDAO.DeleteAsync(id);
				await _commentDAO.CommitTransactionAsync();

				_logger.LogInformation("DeleteAsync comment success");
				return true;
			}
			catch (Exception ex)
			{
				await _commentDAO.RollbackTransactionAsync();
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
				await _commentDAO.BeginTransactionAsync();
				var comment = await _commentDAO.GetByIdAsync(id);
				await _commentDAO.CommitTransactionAsync();

				_logger.LogInformation("Get comment success");
				return comment;
			}
			catch (Exception ex)
			{
				await _commentDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when get comment");
				return null;
			}

		}

		public async Task<Comment> UpdateComment(CommentDto dto)
		{
			try
			{
				await _commentDAO.BeginTransactionAsync();
				var comment = _mapper.Map<Comment>(dto);
				await _commentDAO.UpdateAsync(comment);
				await _commentDAO.CommitTransactionAsync();

				_logger.LogInformation("UpdateAsync comment success");
				return comment;
			}
			catch (Exception ex)
			{
				await _commentDAO.RollbackTransactionAsync();
				_logger.LogError(ex, "Error when UpdateAsync comment");
				return null;
			}
		}
	}
}
