using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Category;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace final_project_be_Tests
{
    public class CategoryRepositoryTests
    {
        private readonly Mock<ICategoryDAO> _categoryDaoMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CategoryRepository>> _loggerMock;
        private readonly CategoryRepository _repository;

        public CategoryRepositoryTests()
        {
            _categoryDaoMock = new Mock<ICategoryDAO>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CategoryRepository>>();
            _repository = new CategoryRepository(
                _categoryDaoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnCategory_WhenSuccess()
        {
            var dto = new CategoryDto { Title = "Test", Description = "Test Desc" };
            var category = new Category { CategoryId = 1, Title = "Test", Description = "Test Desc" };
            _mapperMock.Setup(m => m.Map<Category>(dto)).Returns(category);
            _categoryDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _categoryDaoMock.Setup(d => d.AddAsync(category)).Returns(Task.CompletedTask);
            _categoryDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateCategory(dto);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Title);
            _categoryDaoMock.Verify(d => d.AddAsync(category), Times.Once);
            _categoryDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var dto = new CategoryDto { Title = "Test", Description = "Test Desc" };
            _mapperMock.Setup(m => m.Map<Category>(dto)).Throws(new Exception("Mapping failed"));
            _categoryDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _categoryDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateCategory(dto);

            Assert.Null(result);
            _categoryDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnTrue_WhenSuccess()
        {
            _categoryDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _categoryDaoMock.Setup(d => d.DeleteAsync(1)).Returns(Task.CompletedTask);
            _categoryDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteCategory(1);

            Assert.True(result);
            _categoryDaoMock.Verify(d => d.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnFalseAndRollback_WhenExceptionThrown()
        {
            _categoryDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _categoryDaoMock.Setup(d => d.DeleteAsync(It.IsAny<int>())).Throws(new Exception("Delete failed"));
            _categoryDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteCategory(1);

            Assert.False(result);
            _categoryDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetAllCategory_ShouldReturnPagedResult()
        {
            var data = new List<Category>
            {
                new Category { CategoryId = 1, Title = "A" },
                new Category { CategoryId = 2, Title = "B" },
                new Category { CategoryId = 3, Title = "C" },
            }.AsQueryable();

            _categoryDaoMock.Setup(d => d.GetAll()).Returns(data);

            var result = _repository.GetAllCategory(1, 3);

            Assert.Equal(3, result.TotalCount);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(3, result.PageSize);
            Assert.Equal(3, result.Items.Count());
        }

        [Fact]
        public async Task GetCategory_ShouldReturnCategory_WhenFound()
        {
            var category = new Category { CategoryId = 1, Title = "Cat1" };
            _categoryDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _categoryDaoMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(category);
            _categoryDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetCategory(1);

            Assert.NotNull(result);
            Assert.Equal("Cat1", result.Title);
        }

        [Fact]
        public async Task GetCategory_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            _categoryDaoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _categoryDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetCategory(1);

            Assert.Null(result);
            _categoryDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnUpdatedCategory_WhenSuccess()
        {
            var dto = new CategoryDto { CategoryId = 1, Title = "Updated", Description = "New Desc" };
            var category = new Category { CategoryId = 1, Title = "Old", Description = "Old Desc" };
            _categoryDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _categoryDaoMock.Setup(d => d.GetByIdAsync(dto.CategoryId)).ReturnsAsync(category);
            _mapperMock.Setup(m => m.Map(dto, category)).Verifiable();
            _categoryDaoMock.Setup(d => d.UpdateAsync(category)).Returns(Task.CompletedTask);
            _categoryDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateCategory(dto);

            Assert.NotNull(result);
            Assert.Equal(1, result.CategoryId);
            _categoryDaoMock.Verify(d => d.UpdateAsync(category), Times.Once);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnNullAndRollback_WhenCategoryNotFound()
        {
            var dto = new CategoryDto { CategoryId = 1, Title = "Updated", Description = "New Desc" };
            _categoryDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _categoryDaoMock.Setup(d => d.GetByIdAsync(dto.CategoryId)).ReturnsAsync((Category)null);
            _categoryDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateCategory(dto);

            Assert.Null(result);
            _categoryDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var dto = new CategoryDto { CategoryId = 1, Title = "Updated", Description = "New Desc" };
            _categoryDaoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _categoryDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateCategory(dto);

            Assert.Null(result);
            _categoryDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }
    }
}
