using Bli.Common;
using Bli.EventBus;
using Bli.Infrastructure.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VideoService.Domain.Entities;
using VideoService.Infrastructure;

namespace VideoService.WebAPI.EventHandler
{
    [EventName("Video.Create")]
    public class VideoCreateEventHandler : DynamicIntegrationEventHandler
    {
        private readonly VideoDbContext videoDb;
        private readonly IMongoDatabase _database;
        private readonly ILogger<VideoCreateEventHandler> _logger;

        public VideoCreateEventHandler(IOptionsSnapshot<MongoDbSettings> options,VideoDbContext videoDb, ILogger<VideoCreateEventHandler> logger)
        {
            _database = new MongoClient(options.
               Value.ConnectionString).GetDatabase(options.
               Value.DatabaseName);
            this.videoDb = videoDb;
           this._logger= logger;
        }
        public override async Task HandleDynamic(string eventName, dynamic eventData)
        {
            Video video = new Video
                (
                eventData.Title,
                eventData.Description ,
                eventData.CreateUserId ,
                eventData.CoverUri,
                eventData.VideoType ,
                eventData.VideoSecond,
                eventData.VideoResouce 
                );

          int count = await videoDb.Video.Include(x => x.VideoResouce).Where(x => x.Title == video.Title && x.Description == video.Description
            && x.VideoSecond == video.VideoSecond && x.CreateUserId == video.CreateUserId && x.VideoType == video.VideoType)
                .CountAsync(v => v.VideoResouce.Any(r => video.VideoResouce.Any(x => x.PlayerPath == r.PlayerPath)));
            if (count > 0) {
               return;
            }
            try
            {
                await videoDb.Video.AddAsync(video);
                _database.CreateCollection("dan" + video.Id);
                videoDb.SaveChanges();
            }
            catch (Exception e)
            {
                _logger.LogInterpolatedError($"videoCreate",e);
            }
        }
    }
}
