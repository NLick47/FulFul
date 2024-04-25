using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VideoService.Domain.Entities;
using VideoService.Domain.Entities.Enum;

namespace VideoService.Infrastructure.Response
{
    public class VideoListVm {
        public int Id { get;  set; }
        public string Title { get;  set; }
        public int PlayerCount { get;  set; }
        public double VideoSecond { get;  set; }
        public DateTime CreateTime { get;  set; }

        public string CoverUri { get;  set; }

        public VideoType VideoType { get;  set; }
        public UserVm user { get; set; }
    }
}
