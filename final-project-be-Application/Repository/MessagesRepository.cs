using final_project_be_Application.Interface;
using final_project_be_Domain.DTOs.Message;
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
    public class MessagesRepository : Repository<Messages>, IMessageRepository
    {

        private readonly IMessageDAO _messageDAO;
        private readonly ILogger<MessagesRepository> _logger; 
        public MessagesRepository(IMessageDAO messageDAO, ILogger<MessagesRepository> logger) : base(messageDAO)
        {
            _messageDAO = messageDAO;
            _logger = logger;
        }

        public async Task<List<Messages>> GetConversationAsync(Guid userId, Guid mentorUserId)
        {
            return await _messageDAO.GetMessagesAsync(userId, mentorUserId);
        }

        public async Task<Messages> SendMessageAsync(Guid senderId, Guid receiverId, string content)
        {
            var newMessage = new Messages
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content
            };

            return await _messageDAO.SendMessageAsync(newMessage);

        }
        public async Task<List<ChatPartnerDto>> GetChatPartnersAsync(Guid userId)
        {
            var sentMessages = await _messageDAO.GetSentMessagesAsync(userId);
            var receivedMessages = await _messageDAO.GetReceivedMessagesAsync(userId);

            var allMessages = sentMessages.Concat(receivedMessages);

            var grouped = allMessages
                .GroupBy(m => m.PartnerId)
                .Select(g => new
                {
                    PartnerId = g.Key,
                    LastSentAt = g.Max(x => x.SentAt)
                })
                .ToList();

            var userIds = grouped.Select(g => g.PartnerId);
            var users = await _messageDAO.GetUsersByIdsAsync(userIds);

            var result = grouped
                .Join(users, g => g.PartnerId, u => u.UserId,
                    (g, u) => new ChatPartnerDto
                    {
                        UserId = u.UserId,
                        Avatar = u.UserMetaData.Avatar,
                        FullName = u.UserMetaData.FirstName +" "+ u.UserMetaData.LastName,
                        LastSentAt = g.LastSentAt
                    })
                .OrderByDescending(x => x.LastSentAt)
                .ToList();

            return result;
        }

        public async Task<bool> IsNewChatRoomAsync(Guid userId1, Guid userId2)
        {
            var messages = await _messageDAO.GetMessagesAsync(userId1, userId2);
            return !messages.Any(); // Nếu không có tin nhắn nào giữa 2 người thì là chat room mới
        }
    }
}
