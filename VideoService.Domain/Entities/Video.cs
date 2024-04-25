using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoService.Domain.Entities.Enum;

namespace VideoService.Domain.Entities
{
    public class Video
    {
        public Video() { }
        public Video(string title, string description, long createUserId, string coverUri, VideoType videoType,
            double videoSecond,List<VideoResouce> resouces)
        {
            this.Title = title;
            this.Description = description;
            this.CreateTime = DateTime.Now;
            this.CreateUserId = createUserId;
            this.CoverUri = coverUri.Replace("\\","/");
            this.VideoType = videoType;
            this.VideoSecond= videoSecond;
            this.VideoResouce= resouces;
          
        }

        public List<VideoResouce> VideoResouce { get; private set; }

        public int Id { get; private set; }

        

        public string Title { get;private set; }
        //视频简介
        public string? Description { get; private set; }

        public int PlayerCount { get;private set; }
        
      

        //视频时长毫秒
        public double VideoSecond { get; private set; }

        //视频创建时间
        public DateTime CreateTime { get; private set; }

        //创建的用户
        public long CreateUserId { get; private set; }

        //视频点赞数
        public int LikeCount { get; private set; }

        //封面地址
        public string CoverUri { get; private set; }

        //视频评论
        public List<VideoComment>? Comments { get; private set; }  = new List<VideoComment>();
        
        //视频转发数量
        public int Transpond { get; private set; }

        //视频属性 搬运或自制
        public VideoType VideoType { get; private set; }

        //收藏数量
        public int CollectCount { get;private set; }

      
        public Video SetCoverUri(string url)
        {
            this.CoverUri = url?.Replace("\\","/");
            return this;
        }

      

        

        public Video AddComments(VideoComment comment)
        {
            this.Comments.Add(comment);
            return this;
        }

        public Video RmoveComments(VideoComment comment)
        {
            this.Comments.Remove(comment);
            return this;
        }

        public Video AugmentPlayerCount()
        {
            ++this.PlayerCount;
            return this;
        }
        public Video LikeVideo()
        {
            ++this.LikeCount;
            return this;
        }

        public Video AddCollect()
        {
            ++this.CollectCount;
            return this;
        }

        public Video RemoveCollect()
        {
            if (this.CollectCount > 0)
            {
                --this.CollectCount;
            }
            return this;
        }

        public Video CancelLike()
        {
           if(this.LikeCount > 0)
            {
                --this.LikeCount;
            }
            return this;
        }



    }
}
