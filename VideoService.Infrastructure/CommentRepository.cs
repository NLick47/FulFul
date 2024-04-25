using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoService.Domain.Entities;

namespace VideoService.Infrastructure
{
    public class CommentRepository
    {
        private readonly VideoDbContext videoDb;
        public CommentRepository(VideoDbContext videoDb)
        {
            this.videoDb = videoDb;
        }

        public async Task<(List<VideoComment>, int)> GetCommentsWithRepliesAsync(int videoId, int pageNumber, int pageSize)
        {
            var query = videoDb.VideoComment.Where(c => c.VideoId == videoId).Include(c => c.Replys);
            var totalRecords = await query.CountAsync();
            var comments = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (comments, totalRecords);
        }
    }
}
