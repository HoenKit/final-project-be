using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Report;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace final_project_be_Tests
{
    public class ReportWorkShopRepositoryTests
    {
        private readonly Mock<IReportWorkShopDAO> _workshopDaoMock = new();
        private readonly Mock<IReportDAO> _reportDaoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<ReportWorkShopRepository>> _loggerMock = new();
        private readonly ReportWorkShopRepository _repository;

        public ReportWorkShopRepositoryTests()
        {
            _repository = new ReportWorkShopRepository(
                _workshopDaoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _reportDaoMock.Object
            );
        }

        [Fact]
        public async Task CreateReportWorkShop_ShouldReturnReportWorkShop_WhenSuccess()
        {
            var dto = new ReportWorkShopDto { Content = "Test", UserId = Guid.NewGuid(), WorkShopId = 1 };
            var report = new Report { ReportId = 10, Content = "Test" };
            var reportWorkShop = new ReportWorkShop { ReportId = 10, WorkshopId = 1 };

            _mapperMock.Setup(m => m.Map<Report>(dto)).Returns(report);
            _reportDaoMock.Setup(d => d.AddAsync(report)).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<ReportWorkShop>(dto)).Returns(reportWorkShop);
            _workshopDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _workshopDaoMock.Setup(d => d.AddAsync(reportWorkShop)).Returns(Task.CompletedTask);
            _workshopDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateReportWorkShop(dto);

            Assert.NotNull(result);
            Assert.Equal(10, result.ReportId);
            _workshopDaoMock.Verify(d => d.AddAsync(reportWorkShop), Times.Once);
            _workshopDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateReportWorkShop_ShouldReturnNullAndRollback_WhenReportNotCreated()
        {
            var dto = new ReportWorkShopDto { Content = "Test", UserId = Guid.NewGuid(), WorkShopId = 1 };
            Report report = null;
            _mapperMock.Setup(m => m.Map<Report>(dto)).Returns(report);
            _workshopDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _workshopDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateReportWorkShop(dto);

            Assert.Null(result);
            _workshopDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateReportWorkShop_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var dto = new ReportWorkShopDto { Content = "Test", UserId = Guid.NewGuid(), WorkShopId = 1 };
            _mapperMock.Setup(m => m.Map<Report>(dto)).Throws(new Exception("Mapping failed"));
            _workshopDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _workshopDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateReportWorkShop(dto);

            Assert.Null(result);
            _workshopDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetAllReportWorkShops_ShouldReturnPagedResult()
        {
            var data = new List<ReportWorkShop>
            {
                new ReportWorkShop { ReportId = 1, WorkshopId = 1 },
                new ReportWorkShop { ReportId = 2, WorkshopId = 1 }
            }.AsQueryable();

            _workshopDaoMock.Setup(d => d.GetAll()).Returns(data);

            var result = _repository.GetAllReportWorkShops(1, 2);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
        }

        [Fact]
        public void GetAllReportWorkShops_ShouldReturnEmptyResult_WhenExceptionThrown()
        {
            _workshopDaoMock.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            var result = _repository.GetAllReportWorkShops(1, 2);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetReportWorkShop_ShouldReturnReportWorkShop_WhenFound()
        {
            var reportWorkShop = new ReportWorkShop { ReportId = 1, WorkshopId = 1 };
            _workshopDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _workshopDaoMock.Setup(d => d.GetByReportId(1)).Returns(reportWorkShop);
            _workshopDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetReportWorkShop(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.ReportId);
        }

        [Fact]
        public async Task GetReportWorkShop_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            _workshopDaoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _workshopDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetReportWorkShop(1);

            Assert.Null(result);
            _workshopDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetGroupedReportWorkShops_ShouldReturnGroupedResult()
        {
            var data = new List<ReportWorkShop>
            {
                new ReportWorkShop { ReportId = 1, WorkshopId = 1 },
                new ReportWorkShop { ReportId = 2, WorkshopId = 1 },
                new ReportWorkShop { ReportId = 3, WorkshopId = 2 }
            }.AsQueryable();

            _workshopDaoMock.Setup(d => d.GetAll()).Returns(data);

            var result = _repository.GetGroupedReportWorkShops(1, 2);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(1, result.Items.First().Id);
            Assert.Equal(2, result.Items.First().ReportCount);
        }

        [Fact]
        public async Task DeleteReportsByWorkShopId_ShouldReturnTrue_WhenReportsDeleted()
        {
            var workShopId = 1;
            var reportWorkShops = new List<ReportWorkShop>
            {
                new ReportWorkShop { ReportId = 1, WorkshopId = workShopId },
                new ReportWorkShop { ReportId = 2, WorkshopId = workShopId }
            };
            _reportDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _workshopDaoMock.Setup(d => d.GetByWorkShopId(workShopId)).Returns(reportWorkShops);
            _reportDaoMock.Setup(d => d.DeleteAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
            _reportDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteReportsByWorkShopId(workShopId);

            Assert.True(result);
            _reportDaoMock.Verify(d => d.DeleteAsync(It.IsAny<int>()), Times.Exactly(2));
            _reportDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteReportsByWorkShopId_ShouldReturnFalse_WhenNoReportsFound()
        {
            var workShopId = 1;
            _reportDaoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _workshopDaoMock.Setup(d => d.GetByWorkShopId(workShopId)).Returns(new List<ReportWorkShop>());
            _reportDaoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteReportsByWorkShopId(workShopId);

            Assert.False(result);
            _reportDaoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteReportsByWorkShopId_ShouldReturnFalseAndRollback_WhenExceptionThrown()
        {
            var workShopId = 1;
            _reportDaoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _reportDaoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteReportsByWorkShopId(workShopId);

            Assert.False(result);
            _reportDaoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }
    }
}