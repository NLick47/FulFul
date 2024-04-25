using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoService.Domain.Entities
{
    public class VideoTouristLike
    {
        public int TouristId { get; set; }
        public Tourist Tourist { get; set; }

        public int VideoId { get; set; }
        public Video Video { get; set; }

        public DateTime LikeTime { get; set; }
    }
}
