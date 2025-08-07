using AutoMapper;
using final_project_be_Application.Interface;
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
    public class EventRepository : Repository<Event>, IEventRepository
    {
        private readonly IUserDAO _userDao;
        private readonly IEventDAO _eventDAO;
        private readonly IMapper _mapper;
        private readonly ILogger<EventRepository> _logger;

        public EventRepository(IUserDAO userDao, IEventDAO eventDAO, IMapper mapper, ILogger<EventRepository> logger) : base(eventDAO)
        {
            _userDao = userDao;
            _eventDAO = eventDAO;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<(bool Success, string Message, User? User)> AddPointsAsync(Guid userId, int points)
        {
            var user = await _userDao.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found", null);

            if (user.Turns <= 0)
                return (false, "No turns left", user);

            user.Point += points;
            user.Turns -= 1;

            await _userDao.UpdateAsync(user);
            return (true, "Points added and turn deducted", user);
        }
        public async Task<(bool Success, string Message, User? User)> DailyLoginAsync(Guid userId)
        {
            var user = await _userDao.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found", null);

            var today = DateTime.UtcNow.Date; // hoặc AddHours(7) nếu dùng giờ VN

            if (user.LastDailyLogin.HasValue && user.LastDailyLogin.Value.Date == today)
            {
                return (false, "Already logged in today", user);
            }

            user.Turns += 1;      
            user.LastDailyLogin = DateTime.UtcNow;

            await _userDao.UpdateAsync(user);
            return (true, "Daily login successful", user);
        }

    }
}
