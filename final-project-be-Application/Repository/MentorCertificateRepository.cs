using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Application.Ultils;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Courses;
using final_project_be_Domain.DTOs.Mentor;
using final_project_be_Domain.DTOs.Module;
using final_project_be_Domain.DTOs.Notification;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO;
using final_project_be_Infrastructure.DAO_Interface;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Repository
{
    public class MentorCertificateRepository : Repository<MentorCertificate>, IMentorCertificateRepository
    {
        private readonly IMentorCertificateDAO _mentorCertificateDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<MentorCertificateRepository> _logger;
        private readonly IBlobStorageService _blobStorageService;

        public MentorCertificateRepository(IMentorCertificateDAO mentorCertificateDAO, IMapper mapper, ILogger<MentorCertificateRepository> logger, IBlobStorageService blobStorageService) : base(mentorCertificateDAO)
        {
            _mentorCertificateDAO = mentorCertificateDAO;
            _mapper = mapper;
            _logger = logger;
            _blobStorageService = blobStorageService;
        }

        public async Task<MentorCertificate> CreateMentorCertificate(MentorCertificateDto dto)
        {
            try
            {
                await _mentorCertificateDAO.BeginTransactionAsync();
                var certificate = _mapper.Map<MentorCertificate>(dto);
                if (dto.FileUrl != null && dto.FileUrl.Length > 0)
                {
                    // Generate a unique filename using GUID
                    var fileExtension = Path.GetExtension(dto.FileUrl.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;

                    // Upload to Azure Blob Storage
                    using (var stream = dto.FileUrl.OpenReadStream())
                    {
                        await _blobStorageService.UploadFileAsync(uniqueFileName, stream);
                    }

                    // Store the full URL in the database (UpdateAsync the blob URL with your storage account name)
                    certificate.FileUrl = $"https://finalprojectbestorage.blob.core.windows.net/phronesisfiles/{uniqueFileName}";
                }

                await _mentorCertificateDAO.AddAsync(certificate);
                await _mentorCertificateDAO.CommitTransactionAsync();
                _logger.LogInformation("AddAsync Course success");
                return certificate;
            }
            catch (Exception ex)
            {
                await _mentorCertificateDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding Course");
                return null;
            }
        }



        public async Task<bool> DeleteMentorCertificate(int id)
        {
            try
            {
                await _mentorCertificateDAO.BeginTransactionAsync();
                await _mentorCertificateDAO.DeleteAsync(id);
                await _mentorCertificateDAO.CommitTransactionAsync();

                _logger.LogInformation("DeleteAsync Notification success");
                return true;
            }
            catch (Exception ex)
            {
                await _mentorCertificateDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when delete Notification");
                return false;
            }
        }

        public PageResult<GetMentorCertificateDto> GetAllMentorCertificates(int page, int pageSize)
        {
            try
            {
                var totalCount = _mentorCertificateDAO.GetAll().Count();
                var mentorCertificates = _mentorCertificateDAO.GetAll()
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
                var mentorCertificateDtos = _mapper.Map<List<GetMentorCertificateDto>>(mentorCertificates);

                _logger.LogInformation("Get Certificates success");

                return new PageResult<GetMentorCertificateDto>(mentorCertificateDtos, totalCount, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when getting Certificates");
                return new PageResult<GetMentorCertificateDto>(new List<GetMentorCertificateDto>(), 0, page, pageSize);
            }
        }

        public async Task<ICollection<MentorCertificate>> GetMentorCertificatesByUserId(Guid userId)
        {
            try
            {
                await _mentorCertificateDAO.BeginTransactionAsync();
                // Lấy tất cả certificate từ DAO
                var allCertificates = _mentorCertificateDAO.GetAll(); // Giả sử đây là IEnumerable<MentorCertificate>

                // Filter theo UserId
                var certificates = allCertificates
                    .Where(c => c.Mentor != null && c.Mentor.UserId == userId)
                    .ToList();

                if (!certificates.Any())
                {
                    _logger.LogWarning("No MentorCertificates found for userId {UserId}", userId);
                    return new List<MentorCertificate>(); // trả về list rỗng nếu không tìm thấy
                }

                await _mentorCertificateDAO.CommitTransactionAsync();

                return certificates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching MentorCertificates by UserId");
                await _mentorCertificateDAO.RollbackTransactionAsync();
                return null;
            }
        }


        public async Task<ICollection<GetMentorCertificateDto>> GetAllMentorCertificatesByMentorId(int MentorId)
        {
            try
            {
                await _mentorCertificateDAO.BeginTransactionAsync();

                var certificates = _mentorCertificateDAO.GetAll()
                    .Where(m => m.MentorId == MentorId)
                    .ToList();

                var moduleDtos = _mapper.Map<List<GetMentorCertificateDto>>(certificates);

                await _mentorCertificateDAO.CommitTransactionAsync();

                _logger.LogInformation("Successfully retrieved Certificate for Mentor ID {MentorId}", MentorId);
                return moduleDtos;
            }
            catch (Exception ex)
            {
                await _mentorCertificateDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error while retrieving modules for Mentor ID {MentorId}", MentorId);
                return new List<GetMentorCertificateDto>();
            }
        }

        public async Task<GetMentorCertificateDto> GetMentorCertificate(int id)
        {
            try
            {
                await _mentorCertificateDAO.BeginTransactionAsync();
                var certificate = await _mentorCertificateDAO.GetByIdAsync(id);
                var mentorCertificateDtos = _mapper.Map<GetMentorCertificateDto>(certificate);
                await _mentorCertificateDAO.CommitTransactionAsync();
                _logger.LogInformation("Get Certificates success");
                return mentorCertificateDtos;
            }
            catch (Exception ex)
            {
                await _mentorCertificateDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when getting Certificates");
                return null;
            }
        }

        public async Task<MentorCertificate> UpdateMentorCertificate(MentorCertificateDto dto)
        {
            try
            {
                await _mentorCertificateDAO.BeginTransactionAsync();
                var certificate = await _mentorCertificateDAO.GetByIdAsync(dto.MentorCertificateId);
                if (certificate == null)
                {
                    _logger.LogWarning("Module not found with ID: {Id}", dto.MentorCertificateId);
                    await _mentorCertificateDAO.RollbackTransactionAsync();
                    return null;
                }
                _mapper.Map(dto, certificate);
                await _mentorCertificateDAO.UpdateAsync(certificate);
                await _mentorCertificateDAO.CommitTransactionAsync();
                _logger.LogInformation("UpdateAsync Module success");
                return certificate;
            }
            catch (Exception ex)
            {
                await _mentorCertificateDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when updating Module");
                return null;
            }
        }
    }
}
