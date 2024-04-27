using Amazon.Runtime.Internal;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System.Linq;
using System.Security.Claims;
using VideoService.Domain.Entities;
using VideoService.Infrastructure;
using VideoService.Infrastructure.Request;
using VideoService.Infrastructure.Response;
using static VideoService.Infrastructure.Response.CommentRepose;

namespace VideoService.WebAPI.Controller
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class CommentController : ControllerBase
    {
        private readonly CommentRepository repository;

        private readonly VideoDbContext videoDb;

        private ILogger<CommentController> logger;

        private  IValidator<AddCommentRequest> addCommentvalidator;
        private  IValidator<AddReplyRequest> addReplyvalidator;

      

        private readonly HttpClient client;
        public CommentController(CommentRepository repository, VideoDbContext videoDb, ILogger<CommentController> logger
            , HttpClient client, IValidator<AddReplyRequest> addReplyRequest, IValidator<AddCommentRequest> addComment)
        {
            this.repository = repository;
            this.videoDb = videoDb;
            this.logger = logger;
            this.client = client;
            this.addReplyvalidator = addReplyRequest;
            this.addCommentvalidator = addComment;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> List(int videoId, int pageNumber = 1)
        {
            const int pageSize = 50;
            
            var (comments, totalRecords) = await repository.GetCommentsWithRepliesAsync(videoId, pageNumber, pageSize);
           
           

            List<UserVm> comUser = null;
            List<UserVm> repUser = null;
            var repsIds   = comments?.SelectMany(x => x.Replys)
                .Select(v => v.UserId).ToList() ?? new List<long>();
            var comIds = comments.Select(x => x.UserId).ToList();

            if (repsIds.Count > 0)
            {
                string repUserstr = await client.GetStringAsync("http://http://8.140.19.170/api/user/getusersbyids?ints="
               + JsonConvert.SerializeObject(repsIds));
                repUser = JsonConvert.DeserializeObject<List<UserVm>>(repUserstr);
            }
            if(comIds.Count > 0)
            {
                string comUserstr = await client.GetStringAsync("http://http://8.140.19.170/api/user/getusersbyids?ints="
           + JsonConvert.SerializeObject(comIds));
                comUser = JsonConvert.DeserializeObject<List<UserVm>>(comUserstr);
            }

            var coms = comments.Select(comment => new CommentRepose
            {
                Id = comment.Id,
                Content = comment.Content,
                CreateTime = comment.CreateTime,
                user = comUser.FirstOrDefault(u => u.Id == comment.UserId) ?? new UserVm().Default(), 
                replies = comment.Replys.Select(reply => new CommentRepose.Reply
                {
                    Content = reply.Content,
                    CreateTime = reply.CreateTime,
                    user = repUser.FirstOrDefault(ru => ru.Id == reply.UserId) ?? new UserVm().Default()
                }).ToList()
            }).ToList();

            return Ok(new {data = new { coms, totalRecords } });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AddCommentRequest request)
        {
            if (!addCommentvalidator.Validate(request).IsValid) return BadRequest();
            long user_id =  long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            bool video_exits =  await videoDb.Video.CountAsync(x => x.Id == request.videoId) > 0;
            CommentRepose respose = null;
            UserVm user = null;
            EntityEntry<VideoComment> entity = null;
           if (!video_exits)
            {
                return Ok(new {result = false,mesg = "视频找不到了"});
            }
            try
            {
                entity =  await videoDb.VideoComment.AddAsync(new Domain.Entities.VideoComment(request.text,user_id,request.videoId));
                await videoDb.SaveChangesAsync();
                user = JsonConvert.DeserializeObject<List<UserVm>>(await client.GetStringAsync("http://8.140.19.170/api/user/getusersbyids?ints=[" + user_id + "]"))[0];
            }
            catch (HttpRequestException e)
            {
                user = new UserVm().Default();
            }catch(DbUpdateException e)
            {
                logger.LogInterpolatedError($"AddCommentError{request.videoId}text{request.text}userId{user_id}", e);
                return Ok(new { result = false, mesg = "发生了错误" });
            }
            respose = new CommentRepose()
            {
                Id = entity.Entity.Id,
                Content = entity.Entity.Content,
                user = user,
                CreateTime = entity.Entity.CreateTime
            };
            return Ok(new {result = true,mesg = "发表完成",data = respose });
        }
        [HttpPut("{commendId}")]
        public async Task<IActionResult> Remove(int commendId)
        {
            long user_id = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var comm = await videoDb.VideoComment.Where(x => x.Id == commendId && x.UserId== user_id).SingleOrDefaultAsync();
            if (comm is null) return Ok();
            videoDb.Remove(comm);
            await  videoDb.SaveChangesAsync();
            return Ok();
        }


        [HttpPut("{commendId}")]
        public async Task<IActionResult> RemoveReply(int commendId)
        {
            long user_id = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var comm = await videoDb.CommentReply.Where(x => x.Id == commendId && x.UserId == user_id).SingleOrDefaultAsync();
            if (comm is null) return Ok();
            videoDb.Remove(comm);
            await videoDb.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> AddRep([FromBody] AddReplyRequest request )
        {
            if (!addReplyvalidator.Validate(request).IsValid) return BadRequest();
            long user_id = long.Parse(this.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var comm = await videoDb.VideoComment.Where(x => x.Id==request.commId).SingleOrDefaultAsync();
            UserVm vm = null;
            if (comm is null) return Ok(new { result = false,mesg = "评论不见了"});
            try
            {
                vm = JsonConvert.DeserializeObject<List<UserVm>>(await client.GetStringAsync("http://8.140.19.170/api/user/getusersbyids?ints=[" + user_id + "]"))[0];
            }
            catch (Exception e)
            {
                vm = new UserVm().Default();
                logger.LogInterpolatedError($"用户信息获取错误",e);
            }
            await videoDb.CommentReply.AddAsync(new CommentReply(request.text,request.commId, user_id));
            await videoDb.SaveChangesAsync();
            return Ok(new { result = true,data = new CommentRepose.Reply() {Id= comm.Replys[0].Id,Content = comm.Replys[0].Content,
            CreateTime = comm.Replys[0].CreateTime,user = vm
            } });
        }
    }
}
