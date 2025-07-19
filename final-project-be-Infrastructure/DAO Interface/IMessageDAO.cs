using final_project_be_Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace final_project_be_Infrastructure.DAO_Interface
{
    public interface IMessageDAO : IGenericDAO<Messages>
    {
        Task<List<Messages>> GetMessagesAsync(Guid userId1, Guid userId2);
        Task<Messages> SendMessageAsync(Messages message);
        Task<List<(Guid PartnerId, DateTime SentAt)>> GetSentMessagesAsync(Guid userId);
        Task<List<(Guid PartnerId, DateTime SentAt)>> GetReceivedMessagesAsync(Guid userId);
        Task<List<User>> GetUsersByIdsAsync(IEnumerable<Guid> ids);
        Task<bool> IsNewChatRoomAsync(Guid userId1, Guid userId2);
    }
}
