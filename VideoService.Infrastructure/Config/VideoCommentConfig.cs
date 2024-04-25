using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoService.Domain.Entities;

namespace VideoService.Infrastructure.Config
{
    public class VideoCommentConfig : IEntityTypeConfiguration<VideoComment>
    {
        public void Configure(EntityTypeBuilder<VideoComment> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Replys).WithOne(x => x.VideoComment).HasForeignKey(x => x.VideoCommentId);
            builder.HasMany(x => x.likeUsers).WithOne();
           
        }
    }
}
