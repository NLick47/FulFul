using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VideoService.Domain.Entities;

namespace VideoService.Infrastructure.Config
{
    public class TourisLikeConfigt : IEntityTypeConfiguration<TouristLike>
    {
        public void Configure(EntityTypeBuilder<TouristLike> builder)
        {
            builder.HasIndex(x => x.videoId).IsUnique(false);
        }
    }
}
