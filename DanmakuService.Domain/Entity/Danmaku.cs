using DanmakuService.Domain.Entity.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DanmakuService.Domain.Entity
{
    public class Danmaku
    {
        public string Id { get; set; }

        public DateTime CreateTime { get;private set; }

        public string Text { get;private set; }

        public long CreateUserId { get;private set; }

        public int VideoID { get;private set; }

        public DanPostionType DanPostion { get;private set; }

        public float Time { get;private set; }

        public int Color { get;private set; }

        public Danmaku(string context,long userId, int videoId, float time, DanPostionType danPostion,int color)
        {
            this.CreateTime = DateTime.Now;
            this.Text = context;
           this. CreateUserId = userId;
            this.Time = time;
            this.DanPostion = danPostion;
            this.Color = color;
            this.VideoID = videoId;
            this.Id = Guid.NewGuid().ToString().Replace("-",string.Empty);
           
        }
       
    }


}
