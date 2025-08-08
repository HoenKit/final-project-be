using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Transaction;
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
    public class TransactionRepositoryTests
    {
        private readonly IMapper _mapper;

        public TransactionRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TransactionDto, Transaction>();
            });
            _mapper = config.CreateMapper();
        }
        private IQueryable<Transaction> GetSampleTransactions()
        {
            var user1 = new User { Email = "a@test.com", Phone = "111" };
            var user2 = new User { Email = "b@test.com", Phone = "222" };

            return new List<Transaction>
        {
            new Transaction
            {
                TransactionId = 1,
                UserId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Points = 100,
                PaymentMethod = "Card",
                Status = "Completed",
                Amount = 50,
                OrderCode = "ORD1",
                CreateAt = new DateTime(2025, 1, 1),
                Users = user1
            },
            new Transaction
            {
                TransactionId = 2,
                UserId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Points = 200,
                PaymentMethod = "Cash",
                Status = "Pending",
                Amount = 70,
                OrderCode = "ORD2",
                CreateAt = new DateTime(2025, 2, 1),
                Users = user2
            }
        }.AsQueryable();
        }
        [Fact]
        public async Task CreateTransaction_Success_ReturnsTransaction()
        {
            // Arrange
            var mockDao = new Mock<ITransactionDAO>();
            var mockLogger = new Mock<ILogger<AnswerRepository>>();

            var dto = new TransactionDto
            {
                UserId = Guid.NewGuid(),
                Points = 100,
                PaymentMethod = "Credit Card",
                Status = "Completed",
                Amount = 50,
                OrderCode = "ORD123",
                CreateAt = DateTime.UtcNow
            };

            mockDao.Setup(d => d.AddAsync(It.IsAny<Transaction>()))
                   .Returns(Task.CompletedTask);

            var repo = new TransactionRepository(mockDao.Object, _mapper, mockLogger.Object);

            // Act
            var result = await repo.CreateTransaction(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.UserId, result.UserId);
            Assert.Equal(dto.Amount, result.Amount);
            mockDao.Verify(d => d.BeginTransactionAsync(), Times.Once);
            mockDao.Verify(d => d.AddAsync(It.IsAny<Transaction>()), Times.Once);
            mockDao.Verify(d => d.CommitTransactionAsync(), Times.Once);
            mockLogger.VerifyLog(LogLevel.Information, "AddAsync transaction success", Times.Once());
        }

        [Fact]
        public async Task CreateTransaction_Exception_ReturnsNullAndRollback()
        {
            // Arrange
            var mockDao = new Mock<ITransactionDAO>();
            var mockLogger = new Mock<ILogger<AnswerRepository>>();

            var dto = new TransactionDto
            {
                UserId = Guid.NewGuid(),
                Points = 100,
                PaymentMethod = "Credit Card",
                Status = "Pending",
                Amount = 50,
                OrderCode = "ORD124",
                CreateAt = DateTime.UtcNow
            };

            mockDao.Setup(d => d.AddAsync(It.IsAny<Transaction>()))
                   .ThrowsAsync(new Exception("DB Error"));

            var repo = new TransactionRepository(mockDao.Object, _mapper, mockLogger.Object);

            // Act
            var result = await repo.CreateTransaction(dto);

            // Assert
            Assert.Null(result);
            mockDao.Verify(d => d.RollbackTransactionAsync(), Times.Once);
            mockLogger.VerifyLog(LogLevel.Error, "Error when adding transaction", Times.Once());
        }
        [Fact]
        public void GetAllTransaction_NoFilter_ReturnsAll()
        {
            // Arrange
            var mockDao = new Mock<ITransactionDAO>();
            mockDao.Setup(d => d.GetAll()).Returns(GetSampleTransactions());

            var mockLogger = new Mock<ILogger<AnswerRepository>>();
            var repo = new TransactionRepository(mockDao.Object, _mapper, mockLogger.Object);

            // Act
            var result = repo.GetAllTransaction(1, 10, null, null, null);

            // Assert
            Assert.Equal(2, result.TotalCount);
            Assert.Equal("Pending", result.Items.First().Status); // Descending date
            mockLogger.VerifyLog(LogLevel.Information, "Get filtered transaction success", Times.Once());
        }

        [Fact]
        public void GetAllTransaction_FilterByUserId_ReturnsSingleUser()
        {
            // Arrange
            var mockDao = new Mock<ITransactionDAO>();
            mockDao.Setup(d => d.GetAll()).Returns(GetSampleTransactions());

            var mockLogger = new Mock<ILogger<AnswerRepository>>();
            var repo = new TransactionRepository(mockDao.Object, _mapper, mockLogger.Object);

            var userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Act
            var result = repo.GetAllTransaction(1, 10, userId, null, null);

            // Assert
            Assert.Single(result.Items);
            Assert.All(result.Items, t => Assert.Equal(userId, t.UserId));
        }

        [Fact]
        public void GetAllTransaction_FilterByStatus_ReturnsOnlyMatching()
        {
            // Arrange
            var mockDao = new Mock<ITransactionDAO>();
            mockDao.Setup(d => d.GetAll()).Returns(GetSampleTransactions());

            var mockLogger = new Mock<ILogger<AnswerRepository>>();
            var repo = new TransactionRepository(mockDao.Object, _mapper, mockLogger.Object);

            var statuses = new List<StatusTransactionEnum> { StatusTransactionEnum.Completed };

            // Act
            var result = repo.GetAllTransaction(1, 10, null, null, statuses);

            // Assert
            Assert.Single(result.Items);
            Assert.All(result.Items, t => Assert.Equal("Completed", t.Status));
        }

        [Fact]
        public void GetAllTransaction_SortAscDate_ReturnsOldestFirst()
        {
            // Arrange
            var mockDao = new Mock<ITransactionDAO>();
            mockDao.Setup(d => d.GetAll()).Returns(GetSampleTransactions());

            var mockLogger = new Mock<ILogger<AnswerRepository>>();
            var repo = new TransactionRepository(mockDao.Object, _mapper, mockLogger.Object);

            // Act
            var result = repo.GetAllTransaction(1, 10, null, "asc_date", null);

            // Assert
            Assert.Equal(new DateTime(2025, 1, 1), result.Items.First().CreateAt);
        }

        [Fact]
        public void GetAllTransaction_Exception_ReturnsEmptyList()
        {
            // Arrange
            var mockDao = new Mock<ITransactionDAO>();
            mockDao.Setup(d => d.GetAll()).Throws(new Exception("DB fail"));

            var mockLogger = new Mock<ILogger<AnswerRepository>>();
            var repo = new TransactionRepository(mockDao.Object, _mapper, mockLogger.Object);

            // Act
            var result = repo.GetAllTransaction(1, 10, null, null, null);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
            mockLogger.VerifyLog(LogLevel.Error, "Error when getting filtered transactions", Times.Once());
        }
    }
}
