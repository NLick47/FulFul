using Bli.JWT;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using VideoService.Domain;
using VideoService.Domain.Entities;

namespace VideoService.Infrastructure
{
    public class VideoCommentService : IVideoCommentService
    {
        private readonly VideoDbContext videoDbContext;
        private readonly ITokenService tokenService;
        private readonly IOptions<JWTOptions> optJWT;
        public VideoCommentService(VideoDbContext videoDbContext, ITokenService tokenService, IOptions<JWTOptions> optJWT)
        {
            this.videoDbContext = videoDbContext;
            this.tokenService = tokenService;
            this.optJWT = optJWT;
        }
        public async Task<CommentReply> AddCommentReply(CommentReply comment)
        {
            VideoComment? comme = await videoDbContext.VideoComment.FindAsync(2);
            if(comme is null)
            {
                throw new Exception("评论不存在");
            } 
            comme.AddCommentReply(comment);
             videoDbContext.VideoComment.Update(comme);
            return comment;
        }

        public async Task<VideoComment> AddVideoComment(VideoComment comment)
        {
            Video? video = await videoDbContext.Video.FindAsync(5);
            if(video is null)
            {
                throw new Exception("视频不存在");
            }
            video.AddComments(comment);
            videoDbContext.Video.Update(video);
            return comment;
        }

        public async Task<int> AddVideoLikeCount(long u_id, int commentId)
        {
            VideoComment? comment =  await videoDbContext.VideoComment.Include(x=> x.likeUsers).FirstOrDefaultAsync();
            if(comment is null)
            {
                throw new Exception("评论不存在");
            }
            LikeUser? user = comment.likeUsers.FirstOrDefault(x => x.id == u_id);
            if(user is null)
            {
                comment.likeUsers.Add(new LikeUser(u_id) );
                comment.AddLikeCount();
            }else
            {
                comment.likeUsers.Remove(new LikeUser(u_id));
                comment.MinusLikeCount();
                
            }
            videoDbContext.VideoComment.Update(comment);
            return commentId;
        }

        public bool ValidateToken(string token)
        {
            return tokenService.ValidateToken(token, optJWT.Value);
        }
    }
}
