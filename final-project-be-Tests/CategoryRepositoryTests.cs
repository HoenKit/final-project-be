using AutoMapper;
using final_project_be_Application.Repository;
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
    public class CategoryRepositoryTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetCategory_ShouldReturnCategory()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionCategoryDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CategoryRepository>();
            var repository = new CategoryRepository(dao, new MapperConfiguration(cfg => { }).CreateMapper(), logger);

            var category = new Category
            {
                Title = "Test",
                Description = "Test Desc",
                IsDeleted = false
            };

            context.categories.Add(category);
            await context.SaveChangesAsync();

            var result = await repository.GetCategory(category.CategoryId);

            Assert.NotNull(result);
            Assert.Equal("Test", result.Title);
        }

        [Fact]
        public void GetAllCategory_ShouldReturnPaginatedResult()
        {
            var context = GetInMemoryDbContext();
            var dao = new NoTransactionCategoryDAO(context);
            var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<CategoryRepository>();
            var repository = new CategoryRepository(dao, new MapperConfiguration(cfg => { }).CreateMapper(), logger);

            context.categories.AddRange(
                new Category { Title = "Cat1", Description = "Desc1" },
                new Category { Title = "Cat2", Description = "Desc2" },
                new Category { Title = "Cat3", Description = "Desc3" }
            );
            context.SaveChanges();

            var result = repository.GetAllCategory(page: 1, pageSize: 2);

            Assert.Equal(2, result.Items.Count());
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(1, result.CurrentPage);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(2, result.TotalPages);
        }
    }
}
