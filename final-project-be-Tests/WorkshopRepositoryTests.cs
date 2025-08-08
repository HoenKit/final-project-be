using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Workshop;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Tests
{
    public class WorkshopRepositoryTests
    {
        private readonly Mock<IWorkshopDAO> _workshopDAOMock;
        private readonly IMapper _mapper;
        private readonly Mock<ILogger<WorkshopRepository>> _loggerMock;
        private readonly WorkshopRepository _repository;

        public WorkshopRepositoryTests()
        {
            _workshopDAOMock = new Mock<IWorkshopDAO>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WorkShopCreateDto, WorkShop>();
            });
            _mapper = config.CreateMapper();

            _loggerMock = new Mock<ILogger<WorkshopRepository>>();

            _repository = new WorkshopRepository(
                _workshopDAOMock.Object,
                _mapper,
                _loggerMock.Object
            );
        }

        private IQueryable<WorkShop> GetSampleData()
        {
            return new List<WorkShop>
        {
            new WorkShop { WorkShopId = 1, UpdateAt = new DateTime(2025, 8, 8) },
            new WorkShop { WorkShopId = 2, UpdateAt = new DateTime(2025, 8, 7) },
            new WorkShop { WorkShopId = 3, UpdateAt = new DateTime(2025, 7, 8) }
        }.AsQueryable();
        }

        [Fact]
        public async Task CreateWorkshopAsync_MentorNotExists_ReturnsNull()
        {
            var dto = new WorkShopCreateDto
            {
                MentorId = 1,
                StreamingLink = "https://youtube.com/watch?v=abc123"
            };

            _workshopDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _workshopDAOMock.Setup(d => d.MentorExists(1)).ReturnsAsync(false);

            var result = await _repository.CreateWorkshopAsync(dto);

            Assert.Null(result);
            _workshopDAOMock.Verify(d => d.AddAsync(It.IsAny<WorkShop>()), Times.Never);
            _workshopDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateWorkshopAsync_MentorExists_AddsWorkshop()
        {
            var dto = new WorkShopCreateDto
            {
                MentorId = 1,
                StreamingLink = "https://youtube.com/watch?v=abc123"
            };

            _workshopDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _workshopDAOMock.Setup(d => d.MentorExists(1)).ReturnsAsync(true);
            _workshopDAOMock.Setup(d => d.AddAsync(It.IsAny<WorkShop>())).Returns(Task.CompletedTask);
            _workshopDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateWorkshopAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(dto.MentorId, result.MentorId);
            Assert.NotNull(result.CreateAt);
            Assert.NotNull(result.UpdateAt);
            _workshopDAOMock.Verify(d => d.AddAsync(It.IsAny<WorkShop>()), Times.Once);
            _workshopDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateWorkshopAsync_ExceptionThrown_RollbacksAndReturnsNull()
        {
            var dto = new WorkShopCreateDto
            {
                MentorId = 1,
                StreamingLink = "https://youtube.com/watch?v=abc123"
            };

            _workshopDAOMock.Setup(d => d.BeginTransactionAsync()).ThrowsAsync(new Exception("DB Error"));
            _workshopDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateWorkshopAsync(dto);

            Assert.Null(result);
            _workshopDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetAllWorkshop_NoData_ReturnsEmpty()
        {
            _workshopDAOMock.Setup(d => d.GetAll()).Returns(new List<WorkShop>().AsQueryable());

            var result = _repository.GetAllWorkshop(1, 10);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public void GetAllWorkshop_WithData_ReturnsSortedDescending()
        {
            _workshopDAOMock.Setup(d => d.GetAll()).Returns(GetSampleData());

            var result = _repository.GetAllWorkshop(1, 10);

            Assert.Equal(3, result.TotalCount);
            Assert.Equal(new[] { 1, 2, 3 }, result.Items.Select(w => w.WorkShopId)); 
        }

        [Fact]
        public void GetAllWorkshop_Pagination_WorksCorrectly()
        {
            _workshopDAOMock.Setup(d => d.GetAll()).Returns(GetSampleData());

            var result = _repository.GetAllWorkshop(2, 1);

            Assert.Single(result.Items);
            Assert.Equal(2, result.CurrentPage);
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(2, result.Items.First().WorkShopId);
        }

        [Fact]
        public async Task GetWorkshop_Found_ReturnsWorkshop_AndCommitTransaction()
        {
            var expected = new WorkShop { WorkShopId = 1 };
            _workshopDAOMock.Setup(d => d.GetByIdAsync(1))
                            .ReturnsAsync(expected);

            var result = await _repository.GetWorkshop(1);

            Assert.Equal(expected, result);
            _workshopDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _workshopDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _workshopDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task GetWorkshop_NotFound_ReturnsNull_AndCommitTransaction()
        {
            _workshopDAOMock.Setup(d => d.GetByIdAsync(1))
                            .ReturnsAsync((WorkShop)null);

            var result = await _repository.GetWorkshop(1);

            Assert.Null(result);
            _workshopDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _workshopDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _workshopDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task GetWorkshop_Exception_RollsBackAndReturnsNull()
        {
            _workshopDAOMock.Setup(d => d.GetByIdAsync(1))
                            .ThrowsAsync(new Exception("DB error"));

            var result = await _repository.GetWorkshop(1);

            Assert.Null(result);
            _workshopDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _workshopDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Never);
            _workshopDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateWorkshop_FullUpdate_Success()
        {
            var existing = new WorkShop { WorkShopId = 1, Decription = "Old", StreamingLink = "old-link", CreateAt = DateTime.UtcNow.AddDays(-2) };
            var dto = new WorkShopDto
            {
                WorkShopId = 1,
                Decription = "New Description",
                StreamingLink = "new-link",
                CreateAt = DateTime.UtcNow.AddDays(-10)
            };

            _workshopDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(existing);

            var result = await _repository.UpdateWorkshop(dto);

            Assert.Equal("New Description", result.Decription);
            Assert.Equal("new-link", result.StreamingLink);
            Assert.Equal(dto.CreateAt, result.CreateAt);
            _workshopDAOMock.Verify(d => d.UpdateAsync(existing), Times.Once);
            _workshopDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateWorkshop_PartialUpdate_KeepsOldValues()
        {
            var oldCreateAt = DateTime.UtcNow.AddDays(-5);
            var existing = new WorkShop { WorkShopId = 1, Decription = "Old", StreamingLink = "old-link", CreateAt = oldCreateAt };
            var dto = new WorkShopDto
            {
                WorkShopId = 1,
                Decription = "",  
                StreamingLink = null, 
                CreateAt = default 
            };

            _workshopDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(existing);

            var result = await _repository.UpdateWorkshop(dto);

            Assert.Equal("Old", result.Decription);
            Assert.Equal("old-link", result.StreamingLink);
            Assert.Equal(oldCreateAt, result.CreateAt);
            _workshopDAOMock.Verify(d => d.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task UpdateWorkshop_NotFound_ReturnsNull()
        {
            _workshopDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync((WorkShop)null);

            var result = await _repository.UpdateWorkshop(new WorkShopDto { WorkShopId = 1 });

            Assert.Null(result);
            _workshopDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateWorkshop_Exception_RollsBack()
        {
            _workshopDAOMock.Setup(d => d.GetByIdAsync(1)).ThrowsAsync(new Exception("DB error"));

            var result = await _repository.UpdateWorkshop(new WorkShopDto { WorkShopId = 1 });

            Assert.Null(result);
            _workshopDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteWorkshop_Success_ReturnsTrue()
        {
            // Arrange
            _workshopDAOMock.Setup(d => d.DeleteAsync(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.DeleteWorkshop(1);

            // Assert
            Assert.True(result);
            _workshopDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _workshopDAOMock.Verify(d => d.DeleteAsync(1), Times.Once);
            _workshopDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
            _workshopDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteWorkshop_Exception_ReturnsFalse()
        {
            // Arrange
            _workshopDAOMock
                .Setup(d => d.DeleteAsync(1))
                .ThrowsAsync(new Exception("DB error"));

            // Act
            var result = await _repository.DeleteWorkshop(1);

            // Assert
            Assert.False(result);
            _workshopDAOMock.Verify(d => d.BeginTransactionAsync(), Times.Once);
            _workshopDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Never);
            _workshopDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

    }
}
