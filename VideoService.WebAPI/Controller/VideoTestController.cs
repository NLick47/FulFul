using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VideoService.Domain;
using VideoService.Domain.Entities;
using VideoService.Infrastructure;
using VideoService.WebAPI.Videos.Request;

namespace VideoService.WebAPI.Controller
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class VideoTestController : ControllerBase
    {
        private readonly IVideoCommentService videoCommentService;

       

        private readonly VideoDbContext videoDbContext;

        private readonly CommentReply commentReply;
        public VideoTestController(IVideoCommentService videoCommentService, VideoDbContext videoDbContext, CommentReply commentReply = null)
        {
            this.videoCommentService = videoCommentService;
            this.videoDbContext = videoDbContext;
            this.commentReply = commentReply;
        }

        //上传表单

        
        public string UploadUploadFrom([FromBody] UploadVideoFormRequst _form)
        {
            _form.UserId = long.Parse(this.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            return default;
        }

        //[HttpGet]
        //public async Task<string> AddRei()
        //{
        //    await videoCommentService.AddCommentReply(
        //        new CommentReply() { Content ="hhhh真好玩"}
        //         );
        //    await videoDbContext.SaveChangesAsync();
        //    return "hhhh";
        //}

        public async Task<string> GetVideo()
        {
            var  videoComment = await videoDbContext.Video.Include(x => x.Comments).Where(x => x.Id==2).FirstAsync();
            
            return "";

        }

        [HttpGet]
        public async Task<string> DeleteVideo()
        {
            var video =  videoDbContext.Video.Find(4);
            videoDbContext.Video.Remove(video);
            await videoDbContext.SaveChangesAsync();
            return "Oks";
        }

        [HttpGet]
        public async Task<string> DeleteRei()
        {
            var video = videoDbContext.CommentReply.Find(3);
            videoDbContext.CommentReply.Remove(video);
            await videoDbContext.SaveChangesAsync();
            return "Oks";
        }
    }
}
