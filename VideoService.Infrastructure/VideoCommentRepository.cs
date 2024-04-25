using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VideoService.Domain;
using VideoService.Domain.Entities;

namespace VideoService.Infrastructure
{
    public class VideoCommentRepository : IVideoCommentRepository
    {
        private readonly VideoDbContext videoDb;

        public VideoCommentRepository(VideoDbContext videoDb)
        {
            this.videoDb = videoDb;
        }
        public async Task<IEnumerable<VideoComment>> GetVideoComment(int vi_id)
        {
            var video = await videoDb.Video.Include(x => x.Comments).
                ThenInclude(x => x.Replys).Where(x => x.Id == vi_id).FirstOrDefaultAsync();
            if (video != null) return video.Comments;
            return null;
        }

       
    }
}
