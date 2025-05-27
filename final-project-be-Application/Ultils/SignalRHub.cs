using final_project_be_Domain.DTOs.Comment;
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
    }
}
