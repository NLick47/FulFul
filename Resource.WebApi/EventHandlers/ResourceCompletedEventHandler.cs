using Bli.EventBus;
using Bli.Infrastructure.Options;
using Bli.Infrastructure.Request;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Resource.Infrastructure;
using SixLabors.ImageSharp;
using StackExchange.Redis;

namespace Resource.WebApi.EventHandlers
{
    [EventName("Resource.Completed")]
    public class ResourceCompletedEventHandler : DynamicIntegrationEventHandler
    {
        private readonly IConnectionMultiplexer redisConn;

        private readonly IOptions<FileServiceOptions> fileServer;

        private readonly IConfiguration configuration;

        private readonly ResourceDbContext resourceDb;

        private readonly IHubContext<CombineNotificationHub> hubContext;

        private readonly ILogger<ResourceCompletedEventHandler> logger;
        public ResourceCompletedEventHandler(IConnectionMultiplexer redisConn, IOptions<FileServiceOptions> fileService,
            ILogger<ResourceCompletedEventHandler> logger,IConfiguration configuration,ResourceDbContext resource, IHubContext<CombineNotificationHub> hubContext)
        {
            this.redisConn = redisConn;
            this.fileServer = fileService;
            this.logger = logger;
            this.configuration = configuration;
            this.resourceDb = resource;
            this.hubContext = hubContext;
        }
        public override async Task HandleDynamic(string eventName, dynamic eventData)
        {
            var redisValueDic = new Dictionary<int, Task<RedisValue>>();
            var db = redisConn.GetDatabase();
            string hashform = eventData.formHash;
            var res = JsonConvert.DeserializeObject<UploadVideoFormRequst>(await db.HashGetAsync(hashform,"Form"));
            var complete_path = Path.Combine(fileServer.Value.RootPath, $"{configuration.GetValue<string>("FilePath:VideoPath")}/{hashform}");
            var file_path = Path.Combine(complete_path, $"{hashform}");
            try
            {
                if (!Directory.Exists(complete_path))
                {
                    Directory.CreateDirectory(complete_path);
                    var batch = db.CreateBatch();
                    for (int i = 0; i < res.SliceCount; i++)
                    {
                        var path = batch.HashGetAsync(hashform, i);
                        redisValueDic.Add(i, path);
                    }
                    batch.Execute();

                    using (var fws = new FileStream(file_path, FileMode.CreateNew, FileAccess.Write))
                    {
                        for (int i = 0; i < redisValueDic.Count; i++)
                        {
                            var path = (await redisValueDic[i]).ToString();
                            using (var frs = new FileStream(path, FileMode.Open, FileAccess.Read))
                            {
                                byte[] buffer = new byte[1024];
                                int r;
                                while ((r = await frs.ReadAsync(buffer, 0, buffer.Length)) > 0)
                                {
                                    await fws.WriteAsync(buffer, 0, r);
                                }
                            }
                            await fws.FlushAsync();
                        }
                    }
                    batch = db.CreateBatch();
                    for (int i = 0; i < res.SliceCount; i++)
                    {
                        var resbool = batch.HashDeleteAsync(hashform, i);
                    }
                    batch.Execute();
                    await db.SetAddAsync("video_transcoding_ready", hashform);

                }
            }
            catch (Exception ex)
            {
                logger.LogInterpolatedError($"视频合并失败", ex);
                await hubContext.Clients.User(res.UserId.ToString()).SendAsync("MediaEncoding", res,new {mesg = "视频合并时发生了错误"});
            }
            finally
            {
                try
                {
                    Directory.Delete(Path.Combine(fileServer.Value.RootPath,configuration.GetSection("ChunckVideoSavePath").Value, hashform));
                }
                catch (Exception e)
                {
                    await resourceDb.removeFails.AddAsync(new Entity.RemoveFail(0, eventName, e.Message));
                    resourceDb.SaveChanges();
                    logger.LogInterpolatedError($"片段删除失败", e);
                }
            }
            }
    }
    }
