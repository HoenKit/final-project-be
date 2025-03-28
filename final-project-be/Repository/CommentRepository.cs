using AutoMapper;
using final_project_be.DAO;
using final_project_be.Data;
using final_project_be.Data.Models;
using final_project_be.Dtos.Comment;
using final_project_be.Interface;

namespace final_project_be.Repository
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly CommentDAO commentDAO;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<CommentRepository> _logger;

        public CommentRepository(CommentDAO dao, ApplicationDbContext context, IMapper mapper, ILogger<CommentRepository> logger) : base(dao)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Comment> CreateComment(CommentDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var comment = _mapper.Map<Comment>(dto);
                await commentDAO.AddAsync(comment);

                await transaction.CommitAsync();
                _logger.LogInformation("Add comment success");
                return comment;  
            }
            catch (Exception ex)
            {
                // Rollback nếu lỗi
                await transaction.RollbackAsync();

                // Log lỗi
                _logger.LogError(ex.Message);
                //Console.WriteLine($"Đăng ký thất bại: {ex.Message}");
                return null;
            }
        }
    }
}
