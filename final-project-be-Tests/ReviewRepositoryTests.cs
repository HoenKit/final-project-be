using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Review;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace final_project_be_Tests
{
    public class ReviewRepositoryTests
    {
        private readonly Mock<IReviewDAO> _reviewDaoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<ReviewRepository>> _loggerMock = new();
        private readonly ReviewRepository _repository;

        public ReviewRepositoryTests()
        {
            _repository = new ReviewRepository(
                _reviewDaoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateReview_ShouldReturnReview_WhenSuccess()
        {
            var dto = new ReviewDto { CourseId = 1, UserId = Guid.NewGuid(), Content = "Test", Rate = 5 };
            var review = new Review { ReviewId = 1, Content = "Test", Rate = 5 };
            _mapperMock.Setup(m => m.Map<Review>(dto)).Returns(review);
            _reviewDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _reviewDaoMock.Setup(d => d.AddAsync(review)).Returns(Task.CompletedTask);
            _reviewDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateReview(dto);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Content);
            _reviewDaoMock.Verify(d => d.AddAsync(review), Times.Once);
            _reviewDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateReview_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var dto = new ReviewDto { CourseId = 1, UserId = Guid.NewGuid(), Content = "Test", Rate = 5 };
            _mapperMock.Setup(m => m.Map<Review>(dto)).Throws(new Exception("Mapping failed"));
            _reviewDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _reviewDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateReview(dto);

            Assert.Null(result);
            _reviewDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteReview_ShouldReturnReview_WhenFound()
        {
            var review = new Review { ReviewId = 1, IsDeleted = false, UpdateAt = DateTime.MinValue };
            _reviewDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _reviewDaoMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(review);
            _reviewDaoMock.Setup(d => d.UpdateAsync(review)).Returns(Task.CompletedTask);
            _reviewDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteReview(1);

            Assert.NotNull(result);
            Assert.True(result.IsDeleted);
            _reviewDaoMock.Verify(d => d.UpdateAsync(review), Times.Once);
            _reviewDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteReview_ShouldReturnNullAndRollback_WhenNotFound()
        {
            _reviewDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _reviewDaoMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync((Review)null);
            _reviewDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteReview(1);

            Assert.Null(result);
            _reviewDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteReview_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            _reviewDaoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _reviewDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteReview(1);

            Assert.Null(result);
            _reviewDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetAllReviewsByCourseId_ShouldReturnPagedResult()
        {
            var courseId = 1;
            var userId = Guid.NewGuid();
            var data = new List<Review>
            {
                new Review { ReviewId = 1, CourseId = courseId, UserId = userId, Content = "A", Rate = 4, IsDeleted = false, User = new User() },
                new Review { ReviewId = 2, CourseId = courseId, UserId = userId, Content = "B", Rate = 5, IsDeleted = false, User = new User() }
            }.AsQueryable();

            _reviewDaoMock.Setup(d => d.GetAll()).Returns(data);

            var result = _repository.GetAllReviewsByCourseId(courseId, 1, 2);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.ReviewCount);
            Assert.Equal(4.5m, result.AverageRating);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(1, result.Page);
        }

        [Fact]
        public void GetAllReviewsByCourseId_ShouldReturnEmptyResult_WhenExceptionThrown()
        {
            _reviewDaoMock.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            var result = _repository.GetAllReviewsByCourseId(1, 1, 2);

            Assert.Equal(0, result.TotalCount);
            Assert.Equal(0, result.ReviewCount);
            Assert.Equal(0, result.AverageRating);
            Assert.Empty(result.GetType().GetProperty("reviews")?.GetValue(result) as IEnumerable<ReviewResponseDto> ?? new List<ReviewResponseDto>());
        }

        [Fact]
        public async Task UpdateReview_ShouldReturnReview_WhenFound()
        {
            var dto = new UpdateReviewDto { ReviewId = 1, Content = "Updated", Rate = 5 };
            var review = new Review { ReviewId = 1, Content = "Old", Rate = 3 };
            _reviewDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _reviewDaoMock.Setup(d => d.GetByIdAsync(dto.ReviewId)).ReturnsAsync(review);
            _mapperMock.Setup(m => m.Map(dto, review)).Verifiable();
            _reviewDaoMock.Setup(d => d.UpdateAsync(review)).Returns(Task.CompletedTask);
            _reviewDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateReview(dto);

            Assert.NotNull(result);
            Assert.Equal(1, result.ReviewId);
            _reviewDaoMock.Verify(d => d.UpdateAsync(review), Times.Once);
        }

        [Fact]
        public async Task UpdateReview_ShouldReturnNullAndRollback_WhenNotFound()
        {
            var dto = new UpdateReviewDto { ReviewId = 1, Content = "Updated", Rate = 5 };
            _reviewDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _reviewDaoMock.Setup(d => d.GetByIdAsync(dto.ReviewId)).ReturnsAsync((Review)null);
            _reviewDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateReview(dto);

            Assert.Null(result);
            _reviewDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateReview_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var dto = new UpdateReviewDto { ReviewId = 1, Content = "Updated", Rate = 5 };
            _reviewDaoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _reviewDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateReview(dto);

            Assert.Null(result);
            _reviewDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }
    }
}