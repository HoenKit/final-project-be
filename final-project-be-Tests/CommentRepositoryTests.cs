using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Tests
{
    public class CommentRepositoryTests
    {
        private readonly IMapper _mapper;

        public CommentRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CommentDto, Comment>();
            });
            _mapper = config.CreateMapper();
        }

        private CommentRepository CreateRepository(Mock<ICommentDAO> mockDao)
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CommentRepository>();
            return new CommentRepository(mockDao.Object, _mapper, logger);
        }

        [Fact]
        public async Task CreateComment_ShouldReturnComment()
        {
            // Arrange
            var mockDao = new Mock<ICommentDAO>();
            var dto = new CommentDto { Content = "Test", PostId = 1 };
            var repo = CreateRepository(mockDao);

            mockDao.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.AddAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await repo.CreateComment(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test", result.Content);
            Assert.Equal(1, result.PostId);
        }

        [Fact]
        public async Task DeleteComment_ShouldReturnTrue()
        {
            var mockDao = new Mock<ICommentDAO>();
            var repo = CreateRepository(mockDao);

            mockDao.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.DeleteAsync(1)).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await repo.DeleteComment(1);

            Assert.True(result);
        }

        [Fact]
        public async Task GetComment_ShouldReturnCorrectComment()
        {
            var mockDao = new Mock<ICommentDAO>();
            var comment = new Comment { CommentId = 1, Content = "Fetched", PostId = 2 };
            var repo = CreateRepository(mockDao);

            mockDao.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(comment);
            mockDao.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await repo.GetComment(1);

            Assert.NotNull(result);
            Assert.Equal("Fetched", result.Content);
            Assert.Equal(2, result.PostId);
        }

        [Fact]
        public async Task UpdateComment_ShouldUpdateFields()
        {
            var mockDao = new Mock<ICommentDAO>();
            var dto = new CommentDto { CommentId = 1, Content = "Updated", PostId = 5 };
            var repo = CreateRepository(mockDao);

            mockDao.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.UpdateAsync(It.IsAny<Comment>())).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await repo.UpdateComment(dto);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.Content);
            Assert.Equal(5, result.PostId);
        }

        [Fact]
        public void GetAllComments_ShouldReturnPagedResult()
        {
            var mockDao = new Mock<ICommentDAO>();
            var data = new List<Comment>
        {
            new Comment { CommentId = 1, PostId = 1 },
            new Comment { CommentId = 2, PostId = 1 },
            new Comment { CommentId = 3, PostId = 2 },
        }.AsQueryable();

            mockDao.Setup(d => d.GetAll()).Returns(data);

            var repo = CreateRepository(mockDao);

            var result = repo.GetAllComments(1, 2);

            Assert.Equal(3, result.TotalCount);
        }

        [Fact]
        public void GetAllCommentsByPostId_ShouldReturnCorrectComments()
        {
            var mockDao = new Mock<ICommentDAO>();
            var data = new List<Comment>
        {
            new Comment { CommentId = 1, PostId = 1 },
            new Comment { CommentId = 2, PostId = 1 },
            new Comment { CommentId = 3, PostId = 2 },
        }.AsQueryable();

            mockDao.Setup(d => d.GetAll()).Returns(data);

            var repo = CreateRepository(mockDao);

            var result = repo.GetAllCommentsByPostId(1, 10, 1);

            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Items, c => Assert.Equal(1, c.PostId));
        }
    }
}
