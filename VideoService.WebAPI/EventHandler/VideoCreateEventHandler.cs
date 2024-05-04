using Bli.Common;
using Bli.EventBus;
using Bli.Infrastructure.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Newtonsoft.Json;
using VideoService.Domain.Entities;
using VideoService.Infrastructure;
using VideoService.Infrastructure.Response;

namespace VideoService.WebAPI.EventHandler
{
    [EventName("Video.Create")]
    public class VideoCreateEventHandler : DynamicIntegrationEventHandler
    {
        private readonly VideoDbContext videoDb;
        private readonly IMongoDatabase _database;
        private readonly ILogger<VideoCreateEventHandler> _logger;

        public VideoCreateEventHandler(IMongoDatabase mongo, VideoDbContext videoDb, ILogger<VideoCreateEventHandler> logger)
        {
            _database = mongo;
            this.videoDb = videoDb;
           this._logger= logger;
        }
        public override async Task HandleDynamic(string eventName, dynamic eventData)
        {
            List<VideoResouce> resouce = JsonConvert.DeserializeObject<List<VideoResouce>>(eventData.Resouces);
            Video add = new Video(
                eventData.Title,
                eventData.Description,
                eventData.CreateUserId,
                eventData.CoverUri,
                Enum.Parse<VideoType>(eventData.VideoType),
                eventData.VideoSecond,
                resouce
                );


            var  video  =await videoDb.Video
                .Include(x => x.VideoResouce)
                .Where(x => x.Title == add.Title && x.Description == add.Description
                    && x.VideoSecond == add.VideoSecond && x.CreateUserId == add.CreateUserId && x.VideoType == add.VideoType).SingleOrDefaultAsync();

            if (video != null)
            {
                bool hasSameResources = add.VideoResouce.All(a => video.VideoResouce.Any(v => v.RawPath == a.RawPath));
                if (hasSameResources) return;
            }
            try
            {
                await videoDb.Video.AddAsync(add);
                _database.CreateCollection("dan" + add.Id);
                videoDb.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogInterpolatedError($"videoCreate",e);
            }
        }
    }
}
