using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VideoService.Domain.Entities;

namespace VideoService.Domain
{
    public interface IVideoCommentRepository
    {
     
        Task<IEnumerable<VideoComment>> GetVideoComment(int vi_id);

        
    }
}
