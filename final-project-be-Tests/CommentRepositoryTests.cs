using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.Data;
using final_project_be_Tests.TestDAOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
                cfg.CreateMap<Comment, CommentDto>();
            });
            _mapper = config.CreateMapper();
        }

        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task CreateComment_ShouldAddComment()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionCommentDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CommentRepository>();
            var repository = new CommentRepository(dao, _mapper, logger);

            var dto = new CommentDto
            {
                PostId = 1,
                UserId = Guid.NewGuid(),
                Content = "Test comment"
            };

            var result = await repository.CreateComment(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.Content, result.Content);
        }

        [Fact]
        public async Task DeleteComment_ShouldRemoveComment()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionCommentDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CommentRepository>();
            var repository = new CommentRepository(dao, _mapper, logger);

            var comment = new Comment { PostId = 1, UserId = Guid.NewGuid(), Content = "To delete" };
            context.comments.Add(comment);
            await context.SaveChangesAsync();

            var result = await repository.DeleteComment(comment.CommentId);

            Assert.True(result);
            Assert.Null(await context.comments.FindAsync(comment.CommentId));
        }

        [Fact]
        public async Task GetComment_ShouldReturnCorrectComment()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionCommentDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CommentRepository>();
            var repository = new CommentRepository(dao, _mapper, logger);

            var comment = new Comment { PostId = 1, UserId = Guid.NewGuid(), Content = "Sample" };
            context.comments.Add(comment);
            await context.SaveChangesAsync();

            var result = await repository.GetComment(comment.CommentId);

            Assert.NotNull(result);
            Assert.Equal(comment.Content, result.Content);
        }

        [Fact]
        public async Task UpdateComment_ShouldModifyComment()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionCommentDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CommentRepository>();
            var repository = new CommentRepository(dao, _mapper, logger);

            var comment = new Comment { PostId = 1, UserId = Guid.NewGuid(), Content = "Old content" };
            context.comments.Add(comment);
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var dto = new CommentDto
            {
                CommentId = comment.CommentId,
                PostId = comment.PostId,
                UserId = comment.UserId,
                Content = "Updated content"
            };

            var result = await repository.UpdateComment(dto);

            Assert.NotNull(result);
            Assert.Equal("Updated content", result.Content);
        }

        [Fact]
        public void GetAllCommentsByPostId_ShouldReturnPaginatedComments()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionCommentDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CommentRepository>();
            var repository = new CommentRepository(dao, _mapper, logger);

            context.comments.AddRange(
                new Comment { PostId = 1, UserId = Guid.NewGuid(), Content = "C1" },
                new Comment { PostId = 1, UserId = Guid.NewGuid(), Content = "C2" },
                new Comment { PostId = 2, UserId = Guid.NewGuid(), Content = "C3" }
            );
            context.SaveChanges();

            var result = repository.GetAllCommentsByPostId(page: 1, pageSize: 2, postId: 1);

            Assert.Equal(2, result.Items.Count());
            Assert.All(result.Items, c => Assert.Equal(1, c.PostId));
        }
    }

}
