using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoService.Domain.Entities.Enum;

namespace VideoService.Infrastructure.Response
{
    public class VideoVm
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int PlayerCount { get; set; }
        public double VideoSecond { get; set; }
        public DateTime CreateTime { get; set; }

        public int LikeCount { get;  set; }

        public string Description { get; set; }

        public int CollectCount { get;  set; }

        public int Transpond { get;  set; }

        public string CoverUri { get; set; }

        public VideoType VideoType { get; set; }

        public UserVm user { get; set; }

        public bool isLiked { get; set; }

        public bool isCollected { get; set; }

        public List<VideoResouce> Resouces { get; set; }
        public class VideoResouce
        {
            public string Id { get; set; }
            public string PlayerPath { get;  set; }

            public VideoSize VideoSize { get;  set; }
        }
    }
}
