using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoService.Domain.Entities
{
    public class CommentReply
    {
        public CommentReply(string Content,int VideoCommentId,long UserId)
        {
            this.Content = Content;
            this.VideoCommentId = VideoCommentId;
            this.UserId = UserId;
            this.CreateTime = DateTime.Now;
        }
        public int Id { get; set; }
        public string Content { get;private set; }
        public int VideoCommentId { get;private set; }
        public VideoComment VideoComment { get;private set; }
        
        public long UserId { get;private set; }

        public DateTime CreateTime { get;private set; }
    }
}
