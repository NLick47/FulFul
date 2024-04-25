using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoService.Domain.Entities;

namespace VideoService.Infrastructure.Config
{
    public class VideoConfig : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasMany(x => x.Comments).WithOne(x => x.Video);
            builder.HasMany(x => x.VideoResouce).WithOne(x => x.Video);
        }
    }
}
