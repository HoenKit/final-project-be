using final_project_be_Domain.DTOs.Message;
using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Application.Interface
{
    public interface IMessageRepository : IRepository<Messages>
    {
        public Task<List<Messages>> GetConversationAsync(Guid userId, Guid mentorUserId);
        public Task<Messages> SendMessageAsync(Guid senderId, Guid receiverId, string content);
        public Task<List<ChatPartnerDto>> GetChatPartnersAsync(Guid userId);
        public Task<bool> IsNewChatRoomAsync(Guid userId1, Guid userId2);
    }
}
