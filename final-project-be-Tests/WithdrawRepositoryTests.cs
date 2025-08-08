using AutoMapper;
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
    }
}
