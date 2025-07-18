using final_project_be_Domain.DTOs.Comment;
using final_project_be_Domain.DTOs.Message;
using final_project_be_Domain.DTOs.Post;
using Microsoft.AspNetCore.SignalR;

namespace final_project_be_Application.Ultils
{
    public class SignalRHub : Hub
    {
        public async Task SendPost(PostDto post)
        {
            await Clients.All.SendAsync("ReceivePost", post);
        }

        public async Task SendComment(CommentDto comment)
        {
            await Clients.All.SendAsync("ReceiveComment", comment);
        }
        public async Task SendMessage(MessageDto message)
        {
            await Clients.User(message.ReceiverId.ToString())
                         .SendAsync("ReceiveMessage", message);
        }

        public async Task SendNewChatRoom(Guid receiverId, Guid partnerId)
        {
            await Clients.User(receiverId.ToString())
                         .SendAsync("NewChatRoom", partnerId);
        }
    }
}
