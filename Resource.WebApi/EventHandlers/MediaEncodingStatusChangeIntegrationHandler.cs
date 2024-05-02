using Bli.EventBus;
using Bli.Infrastructure.Request;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Resource.WebApi;
using StackExchange.Redis;

namespace VideoService.WebAPI.EventHandler
{
    [EventName("Resource.Notification")]
    public class MediaEncodingHandler : DynamicIntegrationEventHandler
    {
        private readonly IHubContext<CombineNotificationHub> hubContext;
     
        public MediaEncodingHandler(IHubContext<CombineNotificationHub> hubContext)
        {
            this.hubContext = hubContext;
        }
        public override async Task HandleDynamic(string eventName, dynamic eventData)
        {
            UploadVideoFormRequst form = JsonConvert.DeserializeObject<UploadVideoFormRequst>(eventData.video);
            bool success = eventData.success;
            await hubContext.Clients.User(form.UserId.ToString()).SendAsync("MediaEncoding", form, success);
        }
    }
}
