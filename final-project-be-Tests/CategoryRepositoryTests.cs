using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Category;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using final_project_be_Tests.TestDAOs;
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
    public class CategoryRepositoryTests
    {
        private readonly IMapper _mapper;

        public CategoryRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CategoryDto, Category>();
            });
            _mapper = config.CreateMapper();
        }

        private CategoryRepository CreateRepository(Mock<ICategoryDAO> mockDao)
        {
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CategoryRepository>();
            return new CategoryRepository(mockDao.Object, _mapper, logger);
        }

        [Fact]
        public async Task CreateCategory_ShouldReturnCategory()
        {
            var mockDao = new Mock<ICategoryDAO>();
            var dto = new CategoryDto { Title = "Test", Description = "Test Desc" };
            var repo = CreateRepository(mockDao);

            mockDao.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await repo.CreateCategory(dto);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Title);
        }

        [Fact]
        public async Task DeleteCategory_ShouldReturnTrue()
        {
            var mockDao = new Mock<ICategoryDAO>();
            var repo = CreateRepository(mockDao);

            mockDao.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.DeleteAsync(1)).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await repo.DeleteCategory(1);

            Assert.True(result);
        }

        [Fact]
        public async Task GetCategory_ShouldReturnCorrectCategory()
        {
            var mockDao = new Mock<ICategoryDAO>();
            var category = new Category { CategoryId = 1, Title = "Cat1" };
            var repo = CreateRepository(mockDao);

            mockDao.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(category);
            mockDao.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await repo.GetCategory(1);

            Assert.NotNull(result);
            Assert.Equal("Cat1", result.Title);
        }

        [Fact]
        public async Task UpdateCategory_ShouldReturnUpdatedCategory()
        {
            var mockDao = new Mock<ICategoryDAO>();
            var dto = new CategoryDto { CategoryId = 1, Title = "Updated", Description = "New Desc" };
            var repo = CreateRepository(mockDao);

            mockDao.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.UpdateAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await repo.UpdateCategory(dto);

            Assert.NotNull(result);
            Assert.Equal("Updated", result.Title);
        }

        [Fact]
        public void GetAllCategory_ShouldReturnPagedResult()
        {
            var mockDao = new Mock<ICategoryDAO>();
            var data = new List<Category>
        {
            new Category { CategoryId = 1, Title = "A" },
            new Category { CategoryId = 2, Title = "B" },
            new Category { CategoryId = 3, Title = "C" },
        }.AsQueryable();

            mockDao.Setup(d => d.GetAll()).Returns(data);

            var repo = CreateRepository(mockDao);
            var result = repo.GetAllCategory(1, 3);

            Assert.Equal(3, result.TotalCount);
        }
    }
}
