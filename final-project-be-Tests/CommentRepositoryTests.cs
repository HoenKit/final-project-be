using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace final_project_be_Tests
{
    public class CommentRepositoryTests
    {
        private readonly Mock<ICommentDAO> _commentDaoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CommentRepository>> _loggerMock;
        private readonly CommentRepository _repository;

        public CommentRepositoryTests()
        {
            _commentDaoMock = new Mock<ICommentDAO>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CommentRepository>>();
            _repository = new CommentRepository(
                _commentDaoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateComment_ShouldReturnComment_WhenSuccess()
        {
            var dto = new CommentDto { Content = "Test", PostId = 1, UserId = Guid.NewGuid() };
            var comment = new Comment { CommentId = 1, Content = "Test", PostId = 1, UserId = dto.UserId };
            _mapperMock.Setup(m => m.Map<Comment>(dto)).Returns(comment);
            _commentDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _commentDaoMock.Setup(d => d.AddAsync(comment)).Returns(Task.CompletedTask);
            _commentDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateComment(dto);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Content);
            _commentDaoMock.Verify(d => d.AddAsync(comment), Times.Once);
            _commentDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateComment_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var dto = new CommentDto { Content = "Test", PostId = 1, UserId = Guid.NewGuid() };
            _mapperMock.Setup(m => m.Map<Comment>(dto)).Throws(new Exception("Mapping failed"));
            _commentDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _commentDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateComment(dto);

            Assert.Null(result);
            _commentDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteComment_ShouldReturnTrue_WhenSuccess()
        {
            _commentDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _commentDaoMock.Setup(d => d.DeleteAsync(1)).Returns(Task.CompletedTask);
            _commentDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteComment(1);

            Assert.True(result);
            _commentDaoMock.Verify(d => d.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteComment_ShouldReturnFalseAndRollback_WhenExceptionThrown()
        {
            _commentDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _commentDaoMock.Setup(d => d.DeleteAsync(It.IsAny<int>())).Throws(new Exception("Delete failed"));
            _commentDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteComment(1);

            Assert.False(result);
            _commentDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetAllCommentsByPostId_ShouldReturnPagedResult()
        {
            var postId = 1;
            var data = new List<Comment>
            {
                new Comment { CommentId = 1, PostId = postId, Content = "A" },
                new Comment { CommentId = 2, PostId = postId, Content = "B" },
                new Comment { CommentId = 3, PostId = postId, Content = "C" },
            }.AsQueryable();

            _commentDaoMock.Setup(d => d.GetAll()).Returns(data);

            var result = _repository.GetAllCommentsByPostId(1, 3, postId);

            Assert.Equal(3, result.TotalCount);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(3, result.PageSize);
            Assert.Equal(3, result.Items.Count());
        }

        [Fact]
        public void GetAllCommentsByPostId_ShouldReturnEmptyResult_WhenExceptionThrown()
        {
            _commentDaoMock.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            var result = _repository.GetAllCommentsByPostId(1, 3, 1);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetComment_ShouldReturnComment_WhenFound()
        {
            var comment = new Comment { CommentId = 1, Content = "Test" };
            _commentDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _commentDaoMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(comment);
            _commentDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetComment(1);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Content);
        }

        [Fact]
        public async Task GetComment_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            _commentDaoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _commentDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetComment(1);

            Assert.Null(result);
            _commentDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateComment_ShouldReturnUpdatedComment_WhenSuccess()
        {
            var dto = new CommentDto { CommentId = 1, Content = "Updated", PostId = 1, UserId = Guid.NewGuid() };
            var comment = new Comment { CommentId = 1, Content = "Updated", PostId = 1, UserId = dto.UserId };
            _mapperMock.Setup(m => m.Map<Comment>(dto)).Returns(comment);
            _commentDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _commentDaoMock.Setup(d => d.UpdateAsync(comment)).Returns(Task.CompletedTask);
            _commentDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateComment(dto);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.Content);
            _commentDaoMock.Verify(d => d.UpdateAsync(comment), Times.Once);
        }

        [Fact]
        public async Task UpdateComment_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var dto = new CommentDto { CommentId = 1, Content = "Updated", PostId = 1, UserId = Guid.NewGuid() };
            _mapperMock.Setup(m => m.Map<Comment>(dto)).Throws(new Exception("Mapping failed"));
            _commentDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _commentDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateComment(dto);

            Assert.Null(result);
            _commentDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetAllComments_ShouldReturnPagedResult()
        {
            var data = new List<Comment>
            {
                new Comment { CommentId = 1, Content = "A" },
                new Comment { CommentId = 2, Content = "B" },
                new Comment { CommentId = 3, Content = "C" },
            }.AsQueryable();

            _commentDaoMock.Setup(d => d.GetAll()).Returns(data);

            var result = _repository.GetAllComments(1, 3);

            Assert.Equal(3, result.TotalCount);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(3, result.PageSize);
            Assert.Equal(3, result.Items.Count());
        }

        [Fact]
        public void GetAllComments_ShouldReturnEmptyResult_WhenExceptionThrown()
        {
            _commentDaoMock.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            var result = _repository.GetAllComments(1, 3);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }
    }
}
