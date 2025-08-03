using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Application.Service.EmailService;
using final_project_be_Domain.DTOs.Mentor;
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
    public class MentorRepositoryTests
    {
        private readonly IMapper _mapper;

        public MentorRepositoryTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateMentorDto, Mentor>();
                cfg.CreateMap<Mentor, MentorDto>();
            });
            _mapper = config.CreateMapper();
        }

        [Fact]
        public async Task CreateMentor_ShouldReturnCreatedMentor()
        {
            // Arrange
            var dto = new CreateMentorDto { UserId = Guid.NewGuid(), FirstName = "John", LastName = "Doe" };
            var mockMentorDAO = new Mock<IMentorDAO>();
            var mockUserDAO = new Mock<IUserDAO>();
            var mockEmailService = new Mock<IEmailService>();

            mockUserDAO.Setup(u => u.GetUserMetadatabyId(dto.UserId)).ReturnsAsync(new UserMetadata
            {
                UserId = dto.UserId,
                User = new User
                {
                    UserId = dto.UserId,
                    Email = "mentor@gmail.com"
                }
            });

            mockUserDAO.Setup(u => u.GetRoleByNameAsync("Mentor")).ReturnsAsync(new Role { RoleId = 1, RoleName = "Mentor" });
            mockMentorDAO.Setup(m => m.AddAsync(It.IsAny<Mentor>())).Returns(Task.CompletedTask);
            mockMentorDAO.Setup(m => m.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var repo = new MentorRepository(
                mockMentorDAO.Object,
                Mock.Of<IBlobStorageService>(),
                _mapper,
                Mock.Of<ILogger<MentorRepository>>(),
                Mock.Of<ICourseDAO>(),
                Mock.Of<IReviewDAO>(),
                mockUserDAO.Object,
                mockEmailService.Object);

            // Act
            var result = await repo.CreateMentor(dto);

            // Assert
            Assert.NotNull(result);
            mockMentorDAO.Verify(m => m.AddAsync(It.IsAny<Mentor>()), Times.Once);
            mockMentorDAO.Verify(m => m.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteMentor_ShouldReturnTrue_WhenMentorDeleted()
        {
            // Arrange
            var mockMentorDAO = new Mock<IMentorDAO>();
            mockMentorDAO.Setup(m => m.DeleteAsync(1)).Returns(Task.CompletedTask);

            var repo = new MentorRepository(
                mockMentorDAO.Object,
                Mock.Of<IBlobStorageService>(),
                _mapper,
                Mock.Of<ILogger<MentorRepository>>(),
                Mock.Of<ICourseDAO>(),
                Mock.Of<IReviewDAO>(),
                Mock.Of<IUserDAO>(),
                Mock.Of<IEmailService>());

            // Act
            var result = await repo.DeleteMentor(1);

            // Assert
            Assert.True(result);
            mockMentorDAO.Verify(m => m.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public void GetAllMentors_ShouldReturnPagedList()
        {
            // Arrange
            var mentors = new List<Mentor>
        {
            new Mentor
            {
                MentorId = 1,
                User = new User { UserMetaData = new UserMetadata { FirstName = "Alice", LastName = "Smith" } }
            }
        }.AsQueryable();

            var mockMentorDAO = new Mock<IMentorDAO>();
            mockMentorDAO.Setup(m => m.GetAll()).Returns(mentors);

            var repo = new MentorRepository(
                mockMentorDAO.Object,
                Mock.Of<IBlobStorageService>(),
                _mapper,
                Mock.Of<ILogger<MentorRepository>>(),
                Mock.Of<ICourseDAO>(),
                Mock.Of<IReviewDAO>(),
                Mock.Of<IUserDAO>(),
                Mock.Of<IEmailService>());

            // Act
            var result = repo.GetAllMentors(1, 10);

            // Assert
            Assert.Single(result.Items);
            Assert.Equal(1, result.TotalCount);
        }
        [Fact]
        public async Task UpdateMentor_ShouldUpdateAndReturnMentor()
        {
            // Arrange
            var dto = new CreateMentorDto
            {
                MentorId = 1,
                UserId = Guid.NewGuid(),
                
            };

            var mentorEntity = new Mentor
            {
                MentorId = dto.MentorId,
                UserId = dto.UserId
            };

            var mockDao = new Mock<IMentorDAO>();
            mockDao.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.UpdateAsync(It.IsAny<Mentor>())).Returns(Task.CompletedTask);
            mockDao.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var logger = Mock.Of<ILogger<MentorRepository>>();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<CreateMentorDto, Mentor>()).CreateMapper();
            var repo = new MentorRepository(
                mockDao.Object,
                Mock.Of<IBlobStorageService>(),
                _mapper,
                Mock.Of<ILogger<MentorRepository>>(),
                Mock.Of<ICourseDAO>(),
                Mock.Of<IReviewDAO>(),
                Mock.Of<IUserDAO>(),
                Mock.Of<IEmailService>());

            // Act
            var result = await repo.UpdateMentor(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.MentorId, result.MentorId);
            mockDao.Verify(d => d.UpdateAsync(It.IsAny<Mentor>()), Times.Once);
        }
        [Fact]
        public async Task GetMentorByCourseIdAsync_ShouldReturnMentor()
        {
            // Arrange
            var courseId = 1;
            var mentorDto = new MentorbyCourseDto
            {
                MentorId = 1,
                FirstName = "Hoang",
                LastName = "Nguyen"
            };

            var mockDao = new Mock<IMentorDAO>();
            mockDao.Setup(d => d.GetMentorByCourseIdAsync(courseId)).ReturnsAsync(mentorDto);

            var logger = Mock.Of<ILogger<MentorRepository>>();
            var mapper = Mock.Of<IMapper>();
            var repo = new MentorRepository(
                mockDao.Object,
                Mock.Of<IBlobStorageService>(),
                _mapper,
                Mock.Of<ILogger<MentorRepository>>(),
                Mock.Of<ICourseDAO>(),
                Mock.Of<IReviewDAO>(),
                Mock.Of<IUserDAO>(),
                Mock.Of<IEmailService>());

            // Act
            var result = await repo.GetMentorByCourseIdAsync(courseId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(mentorDto.MentorId, result.MentorId);
        }
        [Fact]
        public async Task UpdateInfoBankAsync_ShouldUpdateMentorBankInfo()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingMentor = new Mentor
            {
                MentorId = 1,
                UserId = userId,
                AccountBank = "OldBank",
                AccountName = "OldName",
                AccountNumber = "00000000"
            };

            var dto = new InfoBank
            {
                AccountBank = "NewBank",
                AccountName = "NewName",
                AccountNumber = "11111111"
            };

            var mockDao = new Mock<IMentorDAO>();
            mockDao.Setup(d => d.GetMentorByUserId(userId)).ReturnsAsync(existingMentor);
            mockDao.Setup(d => d.UpdateAsync(It.IsAny<Mentor>())).Returns(Task.CompletedTask);

            var logger = Mock.Of<ILogger<MentorRepository>>();
            var mapper = Mock.Of<IMapper>();
            var repo = new MentorRepository(mockDao.Object,
                  Mock.Of<IBlobStorageService>(),
                  _mapper,
                  Mock.Of<ILogger<MentorRepository>>(),
                  Mock.Of<ICourseDAO>(),
                  Mock.Of<IReviewDAO>(),
                  Mock.Of<IUserDAO>(),
                  Mock.Of<IEmailService>());

            // Act
            var result = await repo.UpdateInfoBankAsync(userId, dto);

            // Assert
            Assert.True(result);
            mockDao.Verify(d => d.UpdateAsync(It.Is<Mentor>(m =>
                m.AccountBank == "NewBank" &&
                m.AccountName == "NewName" &&
                m.AccountNumber == "11111111"
            )), Times.Once);
        }

    }

}
