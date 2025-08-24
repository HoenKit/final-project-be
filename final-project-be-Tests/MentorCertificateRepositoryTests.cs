using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Repository;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace final_project_be_Tests
{
    public class MentorCertificateRepositoryTests
    {
        private readonly Mock<IMentorCertificateDAO> _daoMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<ILogger<MentorCertificateRepository>> _loggerMock = new();
        private readonly Mock<IBlobStorageService> _blobStorageMock = new();
        private readonly MentorCertificateRepository _repository;

        public MentorCertificateRepositoryTests()
        {
            _repository = new MentorCertificateRepository(
                _daoMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _blobStorageMock.Object
            );
        }

        [Fact]
        public async Task CreateMentorCertificate_ShouldReturnCertificate_WhenSuccessAndFileUploaded()
        {
            var dto = new MentorCertificateDto
            {
                MentorCertificateId = 1,
                MentorId = 2,
                CertificateName = "Cert",
                FileUrl = Mock.Of<IFormFile>(f => f.FileName == "cert.pdf" && f.Length == 1 && f.OpenReadStream() == new MemoryStream(new byte[1]))
            };
            var cert = new MentorCertificate { MentorCertificateId = 1, MentorId = 2, CertificateName = "Cert" };
            _mapperMock.Setup(m => m.Map<MentorCertificate>(dto)).Returns(cert);
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.AddAsync(cert)).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateMentorCertificate(dto);

            Assert.NotNull(result);
            Assert.Equal("Cert", result.CertificateName);
            _blobStorageMock.Verify(b => b.UploadFileAsync(It.IsAny<string>(), It.IsAny<Stream>()), Times.Once);
            _daoMock.Verify(d => d.AddAsync(cert), Times.Once);
            _daoMock.Verify(d => d.CommitTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateMentorCertificate_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var dto = new MentorCertificateDto { MentorCertificateId = 1, MentorId = 2, CertificateName = "Cert" };
            _mapperMock.Setup(m => m.Map<MentorCertificate>(dto)).Throws(new Exception("Mapping failed"));
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.CreateMentorCertificate(dto);

            Assert.Null(result);
            _daoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteMentorCertificate_ShouldReturnTrue_WhenSuccess()
        {
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.DeleteAsync(1)).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteMentorCertificate(1);

            Assert.True(result);
            _daoMock.Verify(d => d.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteMentorCertificate_ShouldReturnFalseAndRollback_WhenExceptionThrown()
        {
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.DeleteAsync(It.IsAny<int>())).Throws(new Exception("Delete failed"));
            _daoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.DeleteMentorCertificate(1);

            Assert.False(result);
            _daoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public void GetAllMentorCertificates_ShouldReturnPagedResult()
        {
            var data = new List<MentorCertificate>
            {
                new MentorCertificate { MentorCertificateId = 1, CertificateName = "A" },
                new MentorCertificate { MentorCertificateId = 2, CertificateName = "B" }
            }.AsQueryable();
            var dtos = new List<GetMentorCertificateDto>
            {
                new GetMentorCertificateDto { MentorCertificateId = 1, CertificateName = "A" },
                new GetMentorCertificateDto { MentorCertificateId = 2, CertificateName = "B" }
            };
            _daoMock.Setup(d => d.GetAll()).Returns(data);
            _mapperMock.Setup(m => m.Map<List<GetMentorCertificateDto>>(It.IsAny<List<MentorCertificate>>())).Returns(dtos);

            var result = _repository.GetAllMentorCertificates(1, 2);

            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Items.Count());
        }

        [Fact]
        public void GetAllMentorCertificates_ShouldReturnEmptyResult_WhenExceptionThrown()
        {
            _daoMock.Setup(d => d.GetAll()).Throws(new Exception("DB error"));

            var result = _repository.GetAllMentorCertificates(1, 2);

            Assert.Empty(result.Items);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task GetAllMentorCertificatesByMentorId_ShouldReturnDtos_WhenSuccess()
        {
            var mentorId = 2;
            var data = new List<MentorCertificate>
            {
                new MentorCertificate { MentorCertificateId = 1, MentorId = mentorId, CertificateName = "A" }
            };
            var dtos = new List<GetMentorCertificateDto>
            {
                new GetMentorCertificateDto { MentorCertificateId = 1, CertificateName = "A" }
            };
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.GetAll()).Returns(data.AsQueryable());
            _mapperMock.Setup(m => m.Map<List<GetMentorCertificateDto>>(It.IsAny<List<MentorCertificate>>())).Returns(dtos);
            _daoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetAllMentorCertificatesByMentorId(mentorId);

            Assert.Single(result);
            Assert.Equal("A", result.First().CertificateName);
        }

        [Fact]
        public async Task GetAllMentorCertificatesByMentorId_ShouldReturnEmptyList_WhenExceptionThrown()
        {
            _daoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _daoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetAllMentorCertificatesByMentorId(1);

            Assert.Empty(result);
            _daoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task GetMentorCertificate_ShouldReturnDto_WhenFound()
        {
            var cert = new MentorCertificate { MentorCertificateId = 1, CertificateName = "A" };
            var dto = new GetMentorCertificateDto { MentorCertificateId = 1, CertificateName = "A" };
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.GetByIdAsync(1)).ReturnsAsync(cert);
            _mapperMock.Setup(m => m.Map<GetMentorCertificateDto>(cert)).Returns(dto);
            _daoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetMentorCertificate(1);

            Assert.NotNull(result);
            Assert.Equal("A", result.CertificateName);
        }

        [Fact]
        public async Task GetMentorCertificate_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            _daoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _daoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.GetMentorCertificate(1);

            Assert.Null(result);
            _daoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateMentorCertificate_ShouldReturnUpdated_WhenFound()
        {
            var dto = new MentorCertificateDto { MentorCertificateId = 1, MentorId = 2, CertificateName = "Updated" };
            var cert = new MentorCertificate { MentorCertificateId = 1, MentorId = 2, CertificateName = "Old" };
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.GetByIdAsync(dto.MentorCertificateId)).ReturnsAsync(cert);
            _mapperMock.Setup(m => m.Map(dto, cert)).Verifiable();
            _daoMock.Setup(d => d.UpdateAsync(cert)).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.CommitTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateMentorCertificate(dto);

            Assert.NotNull(result);
            Assert.Equal(1, result.MentorCertificateId);
            _daoMock.Verify(d => d.UpdateAsync(cert), Times.Once);
        }

        [Fact]
        public async Task UpdateMentorCertificate_ShouldReturnNullAndRollback_WhenNotFound()
        {
            var dto = new MentorCertificateDto { MentorCertificateId = 1, MentorId = 2, CertificateName = "Updated" };
            _daoMock.Setup(d => d.BeginTransactionAsync()).Returns(Task.CompletedTask);
            _daoMock.Setup(d => d.GetByIdAsync(dto.MentorCertificateId)).ReturnsAsync((MentorCertificate)null);
            _daoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateMentorCertificate(dto);

            Assert.Null(result);
            _daoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateMentorCertificate_ShouldReturnNullAndRollback_WhenExceptionThrown()
        {
            var dto = new MentorCertificateDto { MentorCertificateId = 1, MentorId = 2, CertificateName = "Updated" };
            _daoMock.Setup(d => d.BeginTransactionAsync()).Throws(new Exception("DB error"));
            _daoMock.Setup(d => d.RollbackTransactionAsync()).Returns(Task.CompletedTask);

            var result = await _repository.UpdateMentorCertificate(dto);

            Assert.Null(result);
            _daoMock.Verify(d => d.RollbackTransactionAsync(), Times.Once);
        }
    }
}