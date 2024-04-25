using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoService.Domain.Entities.Enum;

namespace VideoService.Domain.Entities
{
    public class VideoResouce
    {
        public VideoResouce(string playerPath,string rawPath,string key,VideoSize videoSize) {
            Id = Guid.NewGuid().ToString().Replace("-",string.Empty);
            this.PlayerPath = playerPath.Replace("\\","/");
            this.RawPath = rawPath;
            this.Key = key;
            this.VideoSize = videoSize;
        }
        public string Id { get;set; } 
        public Video Video { get; set; }
        public int VideoId { get; set; }

        //m3u8地址
        public string PlayerPath { get;private set; }

        //该分辨率转码后的路径
        public string RawPath { get; private set; }

        public VideoResouce SetRawPath(string path)
        {
            this.RawPath = path;
            return this;
        }
      
        public VideoSize VideoSize { get;private set; }
       
        //解密key
        public string? Key { get; private set; }
       

    }
}
