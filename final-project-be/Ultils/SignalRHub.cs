using final_project_be.Dtos.Comment;
using final_project_be.Dtos.Post;
using Microsoft.AspNetCore.SignalR;

namespace final_project_be.Ultils
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
