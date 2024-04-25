using Microsoft.AspNetCore.SignalR;

namespace DanmakuService.WebApi
{
    public class DanmakuHub : Hub
    {
        public async Task SendDanmaku(string videoId, string message)
        {

            await Clients.Group(videoId).SendAsync(videoId, message);
        }

        public async Task SubscribeVideo(string videoId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, videoId);
        }

        public async Task UnsubscribeVideo(string videoId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, videoId);
        }
       
    }
}
