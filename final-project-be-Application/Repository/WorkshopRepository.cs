using AutoMapper;
using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs;
using final_project_be_Domain.DTOs.Workshop;
using final_project_be_Domain.Models;
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
                .OrderByDescending(w => w.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PageResult<WorkShop>(items, totalCount, page, pageSize);
        }

        public async Task<WorkShop> UpdateWorkshop(WorkShopDto dto)
        {
            try
            {
                await _workshopDAO.BeginTransactionAsync();
                var workShop = await _workshopDAO.GetByIdAsync(dto.WorkShopId);
                if (workShop == null)
                {
                    _logger.LogWarning("workShop not found with ID: {Id}", dto.WorkShopId);
                    await _workshopDAO.RollbackTransactionAsync();
                    return null;
                }
                _mapper.Map(dto, workShop);
                await _workshopDAO.UpdateAsync(workShop);
                await _workshopDAO.CommitTransactionAsync();
                _logger.LogInformation("UpdateAsync workShop success");
                return workShop;
            }
            catch (Exception ex)
            {
                await _workshopDAO.RollbackTransactionAsync();
                _logger.LogError(ex, "Error when updating workShop");
                return null;
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
