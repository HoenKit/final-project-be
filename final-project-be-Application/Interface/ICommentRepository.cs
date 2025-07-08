using final_project_be_Domain.Models;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Comment;

namespace final_project_be_Application.Interface
{
    public interface ICommentRepository : IRepository<Comment>
    {
        public Task<Comment> CreateComment(CommentDto dto);
        public Task<bool> DeleteComment(int id);
        public Task<Comment> GetComment(int id);
        public Task<Comment> UpdateComment(CommentDto dto);
        public PageResult<Comment> GetAllCommentsByPostId(int page, int pageSize, int postId);
        public PageResult<Comment> GetAllComments(int page, int pageSize);
    }
}
