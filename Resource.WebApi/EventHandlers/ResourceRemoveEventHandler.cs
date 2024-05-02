using Bli.EventBus;
using Bli.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Resource.Infrastructure;
using Resource.WebApi.Entity;

using System.Security.Cryptography;

namespace Resource.WebApi.EventHandlers
{
    [EventName("ResoueceRemove")]
    [EventName("RawVideoRemove")]
    public class ResourceRemoveEventHandler : DynamicIntegrationEventHandler
    {
        
        private string videoServer;
        private IOptions<FileServiceOptions> fileserver;

        private ILogger<ResourceRemoveEventHandler> logger;

        private readonly ResourceDbContext resourceDb;
        public ResourceRemoveEventHandler(IConfiguration configuration, IOptions<FileServiceOptions> fileserver,
            ILogger<ResourceRemoveEventHandler> logger, ResourceDbContext resourceDb)
        {
            this.videoServer = configuration.GetSection("VideoServer").Value;
            this.fileserver = fileserver;
            this.logger = logger;
            this.resourceDb = resourceDb;
        }
        public override async Task HandleDynamic(string eventName, dynamic eventData)
        {
            int id = eventData.Id;
            try
            {
                switch (eventName)
                {
                    case "ResoueceRemove":
                        Directory.Delete(Path.Combine(fileserver.Value.RootPath, eventData.CoverUri) ,true);
                        foreach (var play in eventData.VideoResouce)
                        {
                            Directory.Delete(play.PlayerPath, true);
                            Directory.Delete(play.RawPath, true);
                        }
                        break;
                    case "RawVideoRemove":
                        Directory.Delete(Path.Combine(fileserver.Value.RootPath,eventData.FilePath));
                        File.Delete(Path.Combine(fileserver.Value.RootPath,eventData.KeyPath));
                        break;
                }
            }
            catch (Exception e)
            {
                logger.LogInterpolatedError($"资源删除{eventName}",e);
                try
                {
                    await resourceDb.removeFails.AddAsync(new Entity.RemoveFail(id, eventName, e.Message));
                    resourceDb.SaveChanges();
                }
                catch (Exception ex)
                {
                    logger.LogInterpolatedError($"addremoveFail",ex);
                }
            }
        }
    }
}
