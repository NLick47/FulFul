using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Security.Claims;
using VideoService.Infrastructure;

namespace VideoService.WebAPI.Controller
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class PlayerController : ControllerBase
    {
        private readonly VideoDbContext videoDbContext;

        private readonly ILogger<PlayerController> logger;

        private readonly IConnectionMultiplexer redisConn;

        private readonly IHttpContextAccessor accessor;
        public PlayerController(VideoDbContext videoDbContext, ILogger<PlayerController> logger,IConnectionMultiplexer connection,
            IHttpContextAccessor accessor)
        {
            this.videoDbContext = videoDbContext;
            this.logger = logger;
            this.redisConn = connection;
            this.accessor = accessor;
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AugmentPlayerCount(int videoId)
        {
            string key_id = this.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? 
                accessor.HttpContext.Connection.RemoteIpAddress.ToString();

            var db = redisConn.GetDatabase();
            bool exits = await db.HashExistsAsync("history_"+key_id,videoId);
            if (exits)
            {
                string lastAccessTime =await db.HashGetAsync("history_" + key_id, videoId.ToString());
                DateTime currentTime = DateTime.Now;
                DateTime lastTime = DateTime.Parse(lastAccessTime);
                if((currentTime - lastTime).TotalMinutes < 45)
                {
                   var video =  await videoDbContext.Video.FirstOrDefaultAsync(x => x.Id == videoId);
                    if (video is null) return BadRequest();
                    videoDbContext.Video.Update(video.AugmentPlayerCount());
                    videoDbContext.SaveChanges();   
                }
            }
            else
            {
                await db.HashSetAsync("history_" + key_id, videoId.ToString(), DateTime.Now.ToString());
                await db.KeyExpireAsync("history_" + key_id, TimeSpan.FromHours(3));
            }
            return Ok();
        }

        /// <summary>
        /// 
        /// 后台任务删除资源
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RemoveResouce([FromBody]Dictionary<string,int> keys)
        {
           long u_id =  long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
           int video_id = keys["videoId"];
           var video = await videoDbContext.Video.FirstOrDefaultAsync(x => x.CreateUserId == u_id && x.Id == video_id);
           if(video is null) return BadRequest(new { mesg = "要删除的资源不存在"});
            videoDbContext.Remove(video);
            videoDbContext.SaveChanges();
            await redisConn.GetDatabase().SetAddAsync("delete_resource",video_id);
            return Ok(new { result = true});
        }

        [HttpGet]
        public async Task<IActionResult> Key(string ott)
        {
            if (string.IsNullOrEmpty(ott) || ott.Length < 32)
            {
                return NotFound();
            }

            var hexkey = await videoDbContext.videoResouces.SingleOrDefaultAsync(x => x.Id == ott);
            if (hexkey == null)
            {
                logger.LogInformation($"No resource found with ID: {ott}");
                return NotFound();
            }

            try
            {
                byte[] keyBytes = Enumerable.Range(0, hexkey.Key.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hexkey.Key.Substring(x, 2), 16))
                    .ToArray();
                string s = BitConverter.ToString(keyBytes);
                return File(keyBytes, "application/octet-stream");

            }
            catch (Exception ex)
            {
                logger.LogError($"Error converting hexkey to bytes: {ex}");
                return StatusCode(500, "Error processing the key");
            }
        }
    }
}
