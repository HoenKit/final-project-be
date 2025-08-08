using AutoMapper;
using Castle.Components.DictionaryAdapter;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs.Withdraw;
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
    public class WithdrawRepositoryTests
    {
        private readonly Mock<IWithdrawDAO> _withdrawDAOMock;
        private readonly Mock<IMentorDAO> _mentorDAOMock;
        private readonly Mock<IUserDAO> _userDAOMock;
        private readonly IMapper _mapper; 
        private readonly Mock<ILogger<WithdrawRepository>> _loggerMock;
        private readonly WithdrawRepository _repository;

        public WithdrawRepositoryTests()
        {
            _withdrawDAOMock = new Mock<IWithdrawDAO>();
            _mentorDAOMock = new Mock<IMentorDAO>();
            _userDAOMock = new Mock<IUserDAO>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<WithdrawDto, Withdraw>();
            });
            _mapper = config.CreateMapper();

            _loggerMock = new Mock<ILogger<WithdrawRepository>>();

            _repository = new WithdrawRepository(
                _withdrawDAOMock.Object,
                _mentorDAOMock.Object,
                _userDAOMock.Object,
                _mapper,
                _loggerMock.Object
            );
        }

        private IQueryable<Withdraw> GetSampleData()
        {
            return new List<Withdraw>
        {
            new Withdraw { MentorId = 1, Status = "Pending", CreateAt = DateTime.UtcNow },
            new Withdraw { MentorId = 1, Status = "Accepted", CreateAt = DateTime.UtcNow.AddDays(-1) },
            new Withdraw { MentorId = 2, Status = "Refused", CreateAt = DateTime.UtcNow.AddMonths(-1) }
        }.AsQueryable();
        }

        [Fact]
        public async Task CreateWithdraw_ShouldReturnWithdraw_WhenSuccess()
        {
            // Arrange
            var dto = new WithdrawDto
            {
                MentorId = 1,
                Points = 100,
                Amount = 50
            };

            _withdrawDAOMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _withdrawDAOMock.Setup(x => x.AddAsync(It.IsAny<Withdraw>())).Returns(Task.CompletedTask);
            _withdrawDAOMock.Setup(x => x.CommitTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateWithdraw(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.MentorId, result.MentorId);
            Assert.Equal(dto.Points, result.Points);
            Assert.Equal(dto.Amount, result.Amount);

            _withdrawDAOMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _withdrawDAOMock.Verify(x => x.AddAsync(It.IsAny<Withdraw>()), Times.Once);
            _withdrawDAOMock.Verify(x => x.CommitTransactionAsync(), Times.Once);
            _withdrawDAOMock.Verify(x => x.RollbackTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateWithdraw_ShouldReturnNull_WhenExceptionThrown()
        {
            // Arrange
            var dto = new WithdrawDto
            {
                MentorId = 1,
                Points = 100,
                Amount = 50
            };

            _withdrawDAOMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _withdrawDAOMock
                .Setup(x => x.AddAsync(It.IsAny<Withdraw>()))
                .ThrowsAsync(new Exception("DB Error"));
            _withdrawDAOMock.Setup(x => x.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _repository.CreateWithdraw(dto);

            // Assert
            Assert.Null(result);

            _withdrawDAOMock.Verify(x => x.BeginTransactionAsync(), Times.Once);
            _withdrawDAOMock.Verify(x => x.AddAsync(It.IsAny<Withdraw>()), Times.Once);
            _withdrawDAOMock.Verify(x => x.CommitTransactionAsync(), Times.Never);
            _withdrawDAOMock.Verify(x => x.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetAllWithdraw_NoFilter_ReturnsAll()
        {
            // Arrange
            _withdrawDAOMock.Setup(x => x.GetAll()).Returns(GetSampleData());

            // Act
            var result = _repository.GetAllWithdraw(1, 10, null, null, null);

            // Assert
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(3, result.Items.Count());
        }

        [Fact]
        public void GetAllWithdraw_FilterByMentorId_ReturnsCorrectData()
        {
            _withdrawDAOMock.Setup(x => x.GetAll()).Returns(GetSampleData());

            var result = _repository.GetAllWithdraw(1, 10, 1, null, null);

            Assert.All(result.Items, w => Assert.Equal(1, w.MentorId));
        }

        [Fact]
        public void GetAllWithdraw_FilterByStatus_ReturnsCorrectData()
        {
            _withdrawDAOMock.Setup(x => x.GetAll()).Returns(GetSampleData());

            var result = _repository.GetAllWithdraw(1, 10, null, null, new List<WithdrawEnum> { WithdrawEnum.Pending });

            Assert.All(result.Items, w => Assert.Equal("Pending", w.Status));
        }

        [Fact]
        public void GetAllWithdraw_FilterCurrentMonth_ReturnsOnlyCurrentMonthData()
        {
            _withdrawDAOMock.Setup(x => x.GetAll()).Returns(GetSampleData());

            var result = _repository.GetAllWithdraw(1, 10, null, null, null, true);

            Assert.All(result.Items, w =>
            {
                Assert.Equal(DateTime.UtcNow.Month, w.CreateAt.Month);
                Assert.Equal(DateTime.UtcNow.Year, w.CreateAt.Year);
            });
        }

        [Fact]
        public void GetAllWithdraw_SortAscDate_ReturnsAscending()
        {
            _withdrawDAOMock.Setup(x => x.GetAll()).Returns(GetSampleData());

            var result = _repository.GetAllWithdraw(1, 10, null, "asc_date", null);

            var sorted = result.Items.OrderBy(x => x.CreateAt).ToList();
            Assert.Equal(sorted, result.Items);
        }

        [Fact]
        public void GetAllWithdraw_Pagination_WorksCorrectly()
        {
            _withdrawDAOMock.Setup(x => x.GetAll()).Returns(GetSampleData());

            var result = _repository.GetAllWithdraw(2, 1, null, null, null);

            Assert.Single(result.Items);
        }
        [Fact]
        public async Task UpdateStatus_WithdrawNotFound_ReturnsNull()
        {
            _withdrawDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _withdrawDAOMock.Setup(d => d.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Withdraw)null);
            _withdrawDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateStatus(1, "Accepted");

            Assert.Null(result);
            _withdrawDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateStatus_NotAcceptedStatus_OnlyUpdatesWithdraw()
        {
            var withdraw = new Withdraw { WithdrawId = 1, Status = "Pending", MentorId = 1, Points = 50 };

            _withdrawDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _withdrawDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(withdraw);
            _withdrawDAOMock.Setup(d => d.UpdateAsync(withdraw)).Returns(Task.CompletedTask);
            _withdrawDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateStatus(1, "Refused");

            Assert.NotNull(result);
            Assert.Equal("Refused", result.Status);
            _userDAOMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateStatus_AcceptedButOldStatusAccepted_NoUserUpdate()
        {
            var withdraw = new Withdraw { WithdrawId = 1, Status = "Accepted", MentorId = 1, Points = 50 };

            _withdrawDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _withdrawDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(withdraw);
            _withdrawDAOMock.Setup(d => d.UpdateAsync(withdraw)).Returns(Task.CompletedTask);
            _withdrawDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateStatus(1, "Accepted");

            Assert.NotNull(result);
            _userDAOMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateStatus_Accepted_UserHasEnoughPoints_UpdatesUserAndWithdraw()
        {
            var withdraw = new Withdraw { WithdrawId = 1, Status = "Pending", MentorId = 1, Points = 50 };
            var user = new User { UserId = Guid.NewGuid(), Point = 100 };

            _withdrawDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _withdrawDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(withdraw);
            _userDAOMock.Setup(u => u.GetUserByMentor(1)).ReturnsAsync(user);
            _userDAOMock.Setup(u => u.UpdateAsync(user)).Returns(Task.CompletedTask);
            _withdrawDAOMock.Setup(d => d.UpdateAsync(withdraw)).Returns(Task.CompletedTask);
            _withdrawDAOMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateStatus(1, "Accepted");

            Assert.NotNull(result);
            Assert.Equal(50, user.Point);
            _userDAOMock.Verify(u => u.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task UpdateStatus_Accepted_UserNotEnoughPoints_Rollbacks()
        {
            var withdraw = new Withdraw { WithdrawId = 1, Status = "Pending", MentorId = 1, Points = 200 };
            var user = new User { UserId = Guid.NewGuid(), Point = 100 };

            _withdrawDAOMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _withdrawDAOMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(withdraw);
            _userDAOMock.Setup(u => u.GetUserByMentor(1)).ReturnsAsync(user);
            _withdrawDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateStatus(1, "Accepted");

            Assert.Null(result);
            _userDAOMock.Verify(u => u.UpdateAsync(It.IsAny<User>()), Times.Never);
            _withdrawDAOMock.Verify(d => d.CommitTransactionAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateStatus_ExceptionThrown_RollbacksAndReturnsNull()
        {
            _withdrawDAOMock.Setup(d => d.BeginTransactionAsync()).ThrowsAsync(new Exception("DB Error"));
            _withdrawDAOMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateStatus(1, "Accepted");

            Assert.Null(result);
            _withdrawDAOMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }
    }
}
