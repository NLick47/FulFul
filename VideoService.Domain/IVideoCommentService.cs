using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VideoService.Domain.Entities;

namespace VideoService.Domain
{
    public interface IVideoCommentService
    {
        public Task<VideoComment> AddVideoComment(VideoComment comment);

        public Task<int> AddVideoLikeCount(long u_id,int commentId);

     
        public Task<CommentReply> AddCommentReply(CommentReply comment);

        public bool ValidateToken(string token);



    }
}
