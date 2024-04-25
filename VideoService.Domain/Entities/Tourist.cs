using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoService.Domain.Entities
{
    public class Tourist
    {
        public int Id { get; set; }
       
        public int Ip { get; set; }

        public List<VideoTouristLike> VideoLikes { get; set; }

    }
}
