using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Category;
using final_project_be_Domain.DTOs.Workshop;
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
    public class WorkshopRepository : Repository<WorkShop>, IWorkshopRepository
    {
        private readonly IWorkshopDAO _workshopDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<WorkshopRepository> _logger;

        public WorkshopRepository(IWorkshopDAO workshopDAO, IMapper mapper, ILogger<WorkshopRepository> logger) : base(workshopDAO) 
        {
            _workshopDAO = workshopDAO;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<WorkShop?> CreateWorkshopAsync(WorkShopCreateDto dto)
        {
            try
            {
                await _workshopDAO.BeginTransactionAsync();
                if (!await _workshopDAO.MentorExists(dto.MentorId))
                {
                    _logger.LogWarning("MentorId {MentorId} not found.", dto.MentorId);
                    return null;
                }
                dto.StreamingLink = ConvertToEmbedLink(dto.StreamingLink);
                var workShop = _mapper.Map<WorkShop>(dto);
                workShop.CreateAt = DateTime.UtcNow;
                workShop.UpdateAt = DateTime.UtcNow;
                await _workshopDAO.AddAsync(workShop);
                await _workshopDAO.CommitTransactionAsync();
                _logger.LogInformation("Add WorkShop success");
                return workShop;
            }
            catch (Exception ex)
            {
                await _workshopDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when adding WorkShop");
                return null;
            }
        }

        public PageResult<WorkShop> GetAllWorkshop(int page, int pageSize)
        {
            var query = _workshopDAO.GetAll();

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(w => w.UpdateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PageResult<WorkShop>(items, totalCount, page, pageSize);
        }
        
        public async Task<WorkShop> GetWorkshop(int id)
        {
            try
            {
                await _workshopDAO.BeginTransactionAsync();
                var lesson = await _workshopDAO.GetByIdAsync(id);
                await _workshopDAO.CommitTransactionAsync();
                _logger.LogInformation("Get Lesson success");
                return lesson;
            }
            catch (Exception ex)
            {
                await _workshopDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when getting Lesson");
                return null;
            }
        }
        public async Task<WorkShop> UpdateWorkshop(WorkShopDto dto)
        {
            try
            {
                await _workshopDAO.BeginTransactionAsync();

                var existing = await _workshopDAO.GetByIdAsync(dto.WorkShopId);
                if (existing == null)
                {
                    _logger.LogWarning("Không tìm thấy Workshop với ID: {0}", dto.WorkShopId);
                    return null;
                }

                existing.Decription = !string.IsNullOrWhiteSpace(dto.Decription)
                    ? dto.Decription
                    : existing.Decription;

                existing.StreamingLink = !string.IsNullOrWhiteSpace(dto.StreamingLink)
                    ? dto.StreamingLink
                    : existing.StreamingLink;


                existing.CreateAt = dto.CreateAt != default(DateTime)
                    ? dto.CreateAt
                    : existing.CreateAt;

                existing.UpdateAt = DateTime.Now;

                await _workshopDAO.UpdateAsync(existing);
                await _workshopDAO.CommitTransactionAsync();

                _logger.LogInformation("Workshop updated successfully with ID: {0}", existing.WorkShopId);
                return existing;
            }
            catch (Exception ex)
            {
                await _workshopDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error updating workshop");
                return null;
            }
        }

        public async Task<bool> DeleteWorkshop(int id)
        {
            try
            {
                await _workshopDAO.BeginTransactionAsync();
                await _workshopDAO.DeleteAsync(id);
                await _workshopDAO.CommitTransactionAsync();

                _logger.LogInformation("DeleteAsync category success");
                return true;
            }
            catch (Exception ex)
            {
                await _workshopDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when delete category");
                return false;
            }
        }

        private string ConvertToEmbedLink(string? url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            if (url.Contains("youtube.com/watch?v="))
            {
                var videoId = url.Split("v=")[1].Split('&')[0];
                return $"https://www.youtube.com/embed/{videoId}";
            }
            else if (url.Contains("youtube.com/live/"))
            {
                var videoId = url.Split("/live/")[1].Split('?')[0];
                return $"https://www.youtube.com/embed/{videoId}";
            }
            else if (url.Contains("youtu.be/"))
            {
                var videoId = url.Split("youtu.be/")[1].Split('?')[0];
                return $"https://www.youtube.com/embed/{videoId}";
            }
            else if (url.Contains("facebook.com/") && url.Contains("/videos/"))
            {
                return url.Replace("facebook.com", "facebook.com/plugins/video.php?href=https://www.facebook.com");
            }
            else if (url.Contains("facebook.com/share/v/")) 
            {
                return url.Replace("facebook.com/share/v/", "facebook.com/plugins/video.php?href=https://www.facebook.com/watch/?v=");
            }

            return url;
        }
    }
}
