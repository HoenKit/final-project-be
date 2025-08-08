using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Post;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Tests
{
    public class PostRepositoryTests
    {
        private readonly Mock<IPostDAO> _postDAOMock;
        private readonly Mock<IPostFileDAO> _postFileDAOMock;
        private readonly Mock<ICommentDAO> _commentDAOMock;
        private readonly Mock<IBlobStorageService> _blobStorageMock;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<PostRepository>> _loggerMock;
        private readonly PostRepository _repository;

        public PostRepositoryTests()
        {
            _postDAOMock = new Mock<IPostDAO>();
            _postFileDAOMock = new Mock<IPostFileDAO>();
            _commentDAOMock = new Mock<ICommentDAO>();
            _blobStorageMock = new Mock<IBlobStorageService>();
            _loggerMock = new Mock<ILogger<PostRepository>>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<PostCreateDto, Post>();
            });
            _mapper = config.CreateMapper();

            _repository = new PostRepository(
                _postDAOMock.Object,
                _postFileDAOMock.Object,
                _commentDAOMock.Object,
                _mapper,
                _loggerMock.Object,
                _blobStorageMock.Object
            );
        }

        private IFormFile CreateMockFormFile(string fileName, int length = 10)
        {
            var ms = new MemoryStream(new byte[length]);
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.Length).Returns(length);
            mockFile.Setup(f => f.OpenReadStream()).Returns(ms);
            return mockFile.Object;
        }

        [Fact]
        public async Task CreatePost_Success_NoFiles_ReturnsPost()
        {
            // Arrange
            var dto = new PostCreateDto
            {
                ParentPostId = 0,
                PostFileLinks = null
            };

            _postDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.AddAsync(It.IsAny<Post>())).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreatePost(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.PostFiles);
            _postDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _postDAOMock.Verify(d => d.AddAsync(It.IsAny<Post>()), Times.Once);
            _postDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _postFileDAOMock.Verify(d => d.AddAsync(It.IsAny<PostFile>()), Times.Never);
            _blobStorageMock.Verify(b => b.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Never);
        }

        [Fact]
        public async Task CreatePost_Exception_RollsBackAndReturnsNull()
        {
            // Arrange
            var dto = new PostCreateDto
            {
                ParentPostId = 0,
                PostFileLinks = null
            };

            _postDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.AddAsync(It.IsAny<Post>())).ThrowsAsync(new Exception("DB error"));
            _postDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreatePost(dto);

            // Assert
            Assert.Null(result);
            _postDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _postDAOMock.Verify(d => d.AddAsync(It.IsAny<Post>()), Times.Once);
            _postDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            _postDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Never);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error when adding Post")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public void GetAllPosts_WithFilters_ReturnsCorrectPageResult()
        {
            // Arrange
            var posts = new List<Post>
    {
        new Post
        {
            PostId = 1,
            Title = "Hello World",
            UserId = Guid.NewGuid(),
            Content = "Content 1",
            CategoryId = 10,
            IsDeleted = false,
            CreateAt = DateTime.UtcNow.AddDays(-1),
            PostFiles = new List<PostFile> { new PostFile { PostFileId = 1, FileUrl = "url1", PostFileType = "Image", IsDeleted = false } },
            Comments = new List<Comment>
            {
                new Comment { CommentId = 1, Content = "Nice post", UserId = Guid.NewGuid() }
            },
            User = new User
            {
                UserId = Guid.NewGuid(),
                UserMetaData = new UserMetadata { FirstName = "Hoang", LastName = "Nguyen" }
            }
        },
        new Post
        {
            PostId = 2,
            Title = "Another Post",
            UserId = Guid.NewGuid(),
            Content = "Content 2",
            CategoryId = 20,
            IsDeleted = true,
            CreateAt = DateTime.UtcNow,
            PostFiles = new List<PostFile>(),
            Comments = new List<Comment>(),
            User = new User()
        }
    }.AsQueryable();

            _postDAOMock.Setup(d => d.GetAll()).Returns(posts);

            // Act
            var result = _repository.GetAllPosts(1, 10, 10, "Hello", null, false);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(10, result.PageSize);

            var postDto = result.Items.First();
            Assert.Equal(1, postDto.PostId);
            Assert.Equal("Hello World", postDto.Title);
            Assert.NotEmpty(postDto.PostFiles);
            Assert.NotEmpty(postDto.Comments);
        }

        [Fact]
        public async Task UpdatePost_ValidDto_UpdatesPostAndFiles()
        {
            // Arrange
            var postId = 1;
            var dto = new PostCreateDto
            {
                PostId = postId,
                Content = "Updated content",
                Title = "Updated title",
                ParentPostId = 0,
                CategoryId = 2,
                PostFileLinks = new List<IFormFile>
            {
                // Mock IFormFile có thể tạo từ MemoryStream, ở đây giả định mock đơn giản
                MockFormFile("file1.jpg"),
                MockFormFile("file2.mp4")
            }
            };

            var existingPost = new Post
            {
                PostId = postId,
                Content = "Old content",
                Title = "Old title",
                ParentPostId = null,
                CategoryId = 1,
                PostFiles = new List<PostFile>(),
                UpdateAt = DateTime.UtcNow.AddDays(-1)
            };

            var existingFiles = new List<PostFile>
        {
            new PostFile { PostFileId = 100, FileUrl = "https://storage/file1.jpg", PostId = postId }
        };

            _postDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.GetByIdAsync(postId)).ReturnsAsync(existingPost);
            _postFileDAOMock.Setup(d => d.GetByPostIdAsync(postId)).ReturnsAsync(existingFiles);
            _postFileDAOMock.Setup(d => d.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            _blobStorageMock.Setup(b => b.DeleteFileIfExistsAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
            _blobStorageMock.Setup(b => b.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>())).Returns(Task.CompletedTask);
            _postFileDAOMock.Setup(d => d.AddAsync(It.IsAny<PostFile>())).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.UpdateAsync(existingPost)).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.UpdatePost(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Content, result.Content);
            Assert.Equal(dto.Title, result.Title);
            Assert.Null(result.ParentPostId); // vì dto.ParentPostId=0 => null
            Assert.Equal(dto.CategoryId, result.CategoryId);
            Assert.True(result.UpdateAt > DateTime.UtcNow.AddMinutes(-5)); // vừa cập nhật

            // Verify các phương thức được gọi
            _postDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _postDAOMock.Verify(d => d.GetByIdAsync(postId), Times.Once);
            _postFileDAOMock.Verify(d => d.GetByPostIdAsync(postId), Times.Once);
            _blobStorageMock.Verify(b => b.DeleteFileIfExistsAsync(It.IsAny<string>()), Times.Exactly(existingFiles.Count));
            _postFileDAOMock.Verify(d => d.DeleteAsync(It.IsAny<int>()), Times.Exactly(existingFiles.Count));
            _blobStorageMock.Verify(b => b.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Exactly(dto.PostFileLinks.Count));
            _postFileDAOMock.Verify(d => d.AddAsync(It.IsAny<PostFile>()), Times.Exactly(dto.PostFileLinks.Count));
            _postDAOMock.Verify(d => d.UpdateAsync(existingPost), Times.Once);
            _postDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdatePost_PostNotFound_ReturnsNullAndRollback()
        {
            // Arrange
            var postId = 99;
            var dto = new PostCreateDto { PostId = postId };

            _postDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.GetByIdAsync(postId)).ReturnsAsync((Post)null);
            _postDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.UpdatePost(dto);

            // Assert
            Assert.Null(result);
            _postDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            _postDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdatePost_ExceptionThrown_ReturnsNullAndRollback()
        {
            // Arrange
            var postId = 1;
            var dto = new PostCreateDto { PostId = postId };

            _postDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.GetByIdAsync(postId)).ThrowsAsync(new Exception("DB error"));
            _postDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.UpdatePost(dto);

            // Assert
            Assert.Null(result);
            _postDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        private static IFormFile MockFormFile(string fileName)
        {
            var content = "Fake file content";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;

            return new FormFile(ms, 0, ms.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = "application/octet-stream"
            };
        }

        [Fact]
        public async Task ToggleIsDeleted_PostExists_TogglesAndReturnsPost()
        {
            // Arrange
            var postId = 1;
            var post = new Post
            {
                PostId = postId,
                IsDeleted = false,
                UpdateAt = DateTime.UtcNow.AddDays(-1)
            };

            _postDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.GetByIdAsync(postId)).ReturnsAsync(post);
            _postDAOMock.Setup(d => d.UpdateAsync(post)).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.ToggleIsDeleted(postId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(postId, result.PostId);
            Assert.True(result.IsDeleted); // trạng thái đã bị toggle
            Assert.True(result.UpdateAt > DateTime.UtcNow.AddMinutes(-1)); // updateAt mới hơn gần đây

            _postDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _postDAOMock.Verify(d => d.GetByIdAsync(postId), Times.Once);
            _postDAOMock.Verify(d => d.UpdateAsync(post), Times.Once);
            _postDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _postDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Never);

            _loggerMock.Verify(
    x => x.Log(
        LogLevel.Information,
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((state, _) => state.ToString().Contains($"Post {postId} banned status changed to True")),
        null,
        It.IsAny<Func<It.IsAnyType, Exception, string>>()),
    Times.Once);

        }

        [Fact]
        public async Task ToggleIsDeleted_PostNotFound_ReturnsNullAndLogsWarning()
        {
            // Arrange
            var postId = 999;

            _postDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.GetByIdAsync(postId)).ReturnsAsync((Post)null);

            // Act
            var result = await _repository.ToggleIsDeleted(postId);

            // Assert
            Assert.Null(result);

            _postDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Never);
            _postDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Never);

            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Post with ID {postId} not found.")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task ToggleIsDeleted_ExceptionOccurs_RollsBackAndReturnsNull()
        {
            // Arrange
            var postId = 1;
            _postDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.GetByIdAsync(postId)).ThrowsAsync(new Exception("DB failure"));
            _postDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.ToggleIsDeleted(postId);

            // Assert
            Assert.Null(result);

            _postDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            _postDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Never);

            _loggerMock.Verify(
    x => x.Log(
        LogLevel.Error,
        It.IsAny<EventId>(),
        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Failed to toggle deleted status for Post {postId}")),
        It.IsAny<Exception>(),
        It.IsAny<Func<It.IsAnyType, Exception, string>>()),
    Times.Once);

        }

        [Fact]
        public async Task GetPostDetail_ExistingPost_ReturnsPostDetailDto()
        {
            // Arrange
            var postId = 1;
            var userId = Guid.NewGuid();

            var posts = new List<Post>
    {
        new Post
        {
            PostId = postId,
            UserId = userId,
            User = new User
            {
                UserId = userId,
                UserMetaData = new UserMetadata
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Avatar = "avatar-url"
                }
            },
            ParentPostId = null,
            Category = new Category { Title = "Tech" },
            IsDeleted = false,
            Title = "Sample Post",
            Content = "Content here",
            CreateAt = DateTime.UtcNow.AddDays(-2),
            UpdateAt = DateTime.UtcNow.AddDays(-1),
            PostFiles = new List<PostFile>
            {
                new PostFile { PostFileId = 1, PostId = postId, FileUrl = "file-url", PostFileType = "Image", IsDeleted = false }
            },
            Comments = new List<Comment>
            {
                new Comment
                {
                    CommentId = 1,
                    PostId = postId,
                    UserId = userId,
                    User = new User
                    {
                        UserId = userId,
                        UserMetaData = new UserMetadata
                        {
                            FirstName = "John",
                            LastName = "Doe",
                            Avatar = "avatar-url"
                        }
                    },
                    ParentCommentId = null,
                    Content = "Nice post!"
                }
            }
        }
    }.AsQueryable();

            // Mock GetAll() to return IQueryable<Post>
            _postDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.GetAll()).Returns(posts);
            _postDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.GetPostDetail(postId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(postId, result.PostId);
            Assert.Equal("John", result.FirstName);
            Assert.Equal("Doe", result.LastName);
            Assert.Equal("Tech", result.CategoryName);
            Assert.False(result.IsDeleted);
            Assert.Single(result.PostFiles);
            Assert.Single(result.Comments);
            Assert.Equal("Nice post!", result.Comments.First().Content);

            _postDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _postDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _postDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task GetPostDetail_PostNotFound_ReturnsNull()
        {
            // Arrange
            var postId = 999;

            _postDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.GetAll()).Returns(new List<Post>().AsQueryable());
            _postDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.GetPostDetail(postId);

            // Assert
            Assert.Null(result);

            _postDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _postDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Never); 
            _postDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task GetPostDetail_ExceptionOccurs_ReturnsNullAndRollsBack()
        {
            // Arrange
            var postId = 1;

            _postDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _postDAOMock.Setup(d => d.GetAll()).Throws(new Exception("DB error"));
            _postDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.GetPostDetail(postId);

            // Assert
            Assert.Null(result);

            _postDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }


    }
}
