using final_project_be.Data.Models;
using final_project_be.Dtos;
using final_project_be.Dtos.Comment;

namespace final_project_be.Interface
{
    public interface ICommentRepository : IRepository<Comment>
    {
        public Task<Comment> CreateComment(CommentDto dto);
        public bool DeleteComment(int id);
        public Task<Comment> GetComment(int id);
        public Task<Comment> UpdateComment(CommentDto dto);
        public PageResult<Comment> GetAllComments(int page, int pageSize);
    }
}
