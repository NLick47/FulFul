using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;


namespace VideoService.Domain.Entities
{
    public class VideoComment
    {
        public VideoComment(string Content,long UserId,int VideoId)
        {
            this.Content = Content;
            this.UserId= UserId;
            this.CreateTime= DateTime.Now;
            this.VideoId= VideoId;
        }
        public int Id { get;private set; }

        public Video Video { get;private set; }

        public int VideoId { get; private set; }
        //评论人
        public long UserId { get;private set; }

        //评论创建时间
        public DateTime CreateTime { get;private set; }
        //评论是否可见
        public bool IsVisible { get; private set; }

        //评论点赞数
        public int LikeCount { get; private set; }

        public List<LikeUser> likeUsers { get; private set; } = new List<LikeUser>();

        //评论内容
        public string Content { get; private set; }

        //评论回复
        public List<CommentReply> Replys { get; private set; } = new List<CommentReply>();

        public VideoComment AddLikeCount()
        {
            this.LikeCount += 1;
            return this;
        }

        public VideoComment MinusLikeCount()
        {
            this.LikeCount -= 1;
            return this;
        }

        public VideoComment AddCommentReply(CommentReply reply)
        {
            Replys.Add(reply);
            return this;
        }
        public VideoComment RemoveCommentReply(CommentReply reply)
        {
            Replys.Remove(reply);
            return this;
        }

      
    }
}
