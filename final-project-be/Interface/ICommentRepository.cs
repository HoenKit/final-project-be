using final_project_be.Data.Models;
using final_project_be.Dtos.Comment;

namespace final_project_be.Interface
{
    public interface ICommentRepository : IRepository<Comment>
    {
        public Task<Comment> CreateComment(CommentDto dto);
    }
}
