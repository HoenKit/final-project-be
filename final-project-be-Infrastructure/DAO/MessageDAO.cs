using final_project_be_Domain.DTOs.Message;
using final_project_be_Domain.Models;
using final_project_be_Infrastructure.DAO_Interface;
using final_project_be_Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO
{
    public class MessageDAO : GenericDAO<Messages>, IMessageDAO
    {
        private readonly ApplicationDbContext _context;
        public MessageDAO(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Messages>> GetMessagesAsync(Guid userId1, Guid userId2)
        {
            return await _context.Messages
                .Where(m =>
                    (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                    (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
        public async Task<Messages> SendMessageAsync(Messages message)
        {
            message.SentAt = DateTime.UtcNow;
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }
        public async Task<List<(Guid PartnerId, DateTime SentAt)>> GetSentMessagesAsync(Guid userId)
        {
            return await _context.Messages
                .Where(m => m.SenderId == userId)
                .Select(m => new ValueTuple<Guid, DateTime>(m.ReceiverId, m.SentAt))
                .ToListAsync();
        }

        public async Task<List<(Guid PartnerId, DateTime SentAt)>> GetReceivedMessagesAsync(Guid userId)
        {
            return await _context.Messages
                .Where(m => m.ReceiverId == userId)
                .Select(m => new ValueTuple<Guid, DateTime>(m.SenderId, m.SentAt))
                .ToListAsync();
        }

        public async Task<List<User>> GetUsersByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.users
                .Where(u => ids.Contains(u.UserId))
                .Include(um=> um.UserMetaData)
                .ToListAsync();
        }

        public async Task<bool> IsNewChatRoomAsync(Guid userId1, Guid userId2)
        {
            return !await _context.Messages.AnyAsync(m =>
                (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                (m.SenderId == userId2 && m.ReceiverId == userId1));
        }
    }
}
