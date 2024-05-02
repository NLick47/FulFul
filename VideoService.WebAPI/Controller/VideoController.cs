using Bli.Common;
using Bli.EventBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using System.Security.Claims;
using System.Text;
using VideoService.Domain;
using VideoService.Domain.Entities;
using VideoService.Infrastructure;
using VideoService.Infrastructure.Migrations;
using VideoService.Infrastructure.Response;



namespace VideoService.WebAPI.Controller
{
   
    [Route("/api/[controller]/[action]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
      
        private readonly VideoDbContext videoDbContext;
        private readonly HttpClient client;
        private readonly IHttpContextAccessor accessor;

        private readonly ILogger<VideoController> _logger;
        private readonly IConfiguration configuration;
        private readonly IEventBus eventBus;

        public VideoController( VideoDbContext videoDbContext,
            ILogger<VideoController> logger,HttpClient client,IHttpContextAccessor accessor,IConfiguration configuration,
            IEventBus bus)
        {
            this.videoDbContext = videoDbContext;
            this._logger = logger;
            this.client = client;
            this.accessor = accessor;
            this.configuration = configuration;
            this.eventBus = bus;
        }
      
        [HttpGet]
        public async Task<IActionResult> VideoList(int curr_page,bool recom = true)
        {
            string token =HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var us = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            const int PageSize = 10;
            
            IQueryable<Video> query = videoDbContext.Video;       
            if (us is null)
            {
                recom = true;
                curr_page = 1;
            }
            if (recom)
            {
                query = query.OrderByDescending(x => x.LikeCount).ThenByDescending(x => x.PlayerCount);
            }
            List<Video> videos = await query.Skip(PageSize * (curr_page - 1)).Take(PageSize).Include(x => x.VideoResouce).ToListAsync();
            if(videos.Count == 0) return Ok(new { result = true, mesg = "没有更多了" });
            List<long> ids = videos.Select(x => x.CreateUserId).ToList();
   
            string res = await client.GetStringAsync($"{configuration.GetSection("UserServer").Value}/getusersbyids?ints=" + JsonConvert.SerializeObject(ids));
            List<UserVm> vms = JsonConvert.DeserializeObject<List<UserVm>>(res);  
            List<VideoListVm> videoVms = videos
                .Zip(vms, (video, user) => new VideoListVm() { Title = video.Title,CreateTime = video.CreateTime
                ,PlayerCount= video.PlayerCount,VideoSecond = video.VideoSecond
                ,CoverUri= video.CoverUri,VideoType = video.VideoType,Id = video.Id
                , user = user }).ToList();

            return Ok(new { result = true,data = videoVms });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideo(int id)
        {
            Video video = await videoDbContext.Video.Include(x => x.VideoResouce).SingleOrDefaultAsync(x => x.Id == id);
            if (video is null) return Ok(new {result = false, mesg ="视频找不到了"});
            var us = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
           
            bool isLikeed = false;
            bool isColled = false;
            if (us is null)
            {
                int ip = ConvertHelper.Ipv4ToInt(accessor.HttpContext.Connection.RemoteIpAddress);
                isLikeed = await videoDbContext.TouristLike.AnyAsync(x => x.videoId == id && x.Ip == ip);
            }
            else
            { 
              long  us_id = long.Parse(us);
                 isColled = await videoDbContext.Collections
                            .Include(x => x.Item)
                        .Where(x => x.CreateUserId == us_id)
                        .Where(x => x.Item.Any(item => item.VideoId == id))
                        .CountAsync() > 0;
                isLikeed = await videoDbContext.VideoLikes.AnyAsync(x => x.VideoId == id && x.UserId == us_id);
            }
           
           
            UserVm userVm = null;
            try
            {
                string res = await client.GetStringAsync($"{configuration.GetSection("UserServer").Value}/getusersbyids?ints=[" + JsonConvert.SerializeObject(video.CreateUserId) + "]");
                userVm = JsonConvert.DeserializeObject<List<UserVm>>(res)[0];
            }
            catch (Exception e)
            {
                _logger.LogError("用户信息获取错误" + e);
                userVm = new UserVm().Default();
            }
           
            var vm = new VideoVm()
            {
                Title= video.Title,
                CoverUri= video.CoverUri,
                isLiked= isLikeed,
                isCollected = isColled,
                CreateTime = video.CreateTime,
                PlayerCount = video.PlayerCount,
                VideoType = video.VideoType,
                user = userVm,
                Id = video.Id,
                LikeCount = video.LikeCount,
                CollectCount = video.CollectCount,
                Transpond = video.Transpond,
                Description = video.Description,
                Resouces =  video.VideoResouce.Select(x => new VideoVm.VideoResouce() { Id=x.Id,VideoSize=x.VideoSize,PlayerPath=x.PlayerPath }).ToList()
            };
            return Ok(new { result = true, data = vm });
        }
      
       

        [HttpPut("{videoId}")]
        public async Task<IActionResult> Like(int videoId)
        {
            var us = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Video video = await videoDbContext.Video.SingleAsync(x => x.Id == videoId);
            long us_id = -1;
            await videoDbContext.Database.BeginTransactionAsync();
            try
            {
                if (us is null)
                {
                   int ip = ConvertHelper.Ipv4ToInt(accessor.HttpContext.Connection.RemoteIpAddress);
                    var tour_like = await videoDbContext.TouristLike.SingleOrDefaultAsync(x => x.Ip == ip && x.videoId == videoId);
                    if(tour_like is null)
                    {
                        await  videoDbContext.TouristLike.AddAsync(new TouristLike {Ip=ip,videoId=videoId });
                        video.LikeVideo();

                    }else
                    {
                         videoDbContext.TouristLike.Remove(tour_like);
                        video.CancelLike();
                    }
                }
                else
                {
                    us_id = long.Parse(us);
                    VideoLike? like = await videoDbContext.VideoLikes.FirstOrDefaultAsync(x => x.VideoId == videoId && x.UserId == us_id);

                    if (like is null)
                    {
                        video.LikeVideo();
                        await videoDbContext.VideoLikes.AddAsync(new VideoLike { UserId = us_id, VideoId = videoId });
                    }
                    else
                    {
                        video.CancelLike();
                        videoDbContext.VideoLikes.Remove(like);
                    }
                }
                videoDbContext.Video.Update(video);
                await videoDbContext.SaveChangesAsync();
                await videoDbContext.Database.CommitTransactionAsync();
            }
            catch (Exception e)
            {
                await videoDbContext.Database.RollbackTransactionAsync();
                return BadRequest("系统miss了你的点赞");
            }
            return Ok(new { data = video });
        }


        [HttpGet("{uid}")]
        public async Task<IActionResult> ListByUsId(long uid)
        {
            var videos = await  videoDbContext.Video.Where(x => x.CreateUserId == uid).ToListAsync();
            List<VideoListVm> videoVms = videos
               .Zip(videos, (video, user) => new VideoListVm()
               {
                   Title = video.Title,
                   CreateTime = video.CreateTime
               ,
                   PlayerCount = video.PlayerCount,
                   VideoSecond = video.VideoSecond
               ,
                   CoverUri = video.CoverUri,
                   VideoType = video.VideoType,
                   Id = video.Id
               }).ToList();

            return Ok(new { result = true,data = videoVms });
        }

      
        [HttpPost]
        public async Task<IActionResult> RemoveVideo([FromBody] Dictionary<string, int> keys)
        {
            long u_id = long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            int video_id = keys["videoId"];
            var video = await videoDbContext.Video.FirstOrDefaultAsync(x => x.CreateUserId == u_id && x.Id == video_id);
            if (video is null) return BadRequest(new { mesg = "要删除的资源不存在" });
            videoDbContext.Remove(video);
            videoDbContext.SaveChanges();
            eventBus.Publish("ResoueceRemove",video);
            return Ok(new { result = true });
        }
    }
}
