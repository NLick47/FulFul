using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoService.Domain.Entities;

namespace VideoService.Infrastructure.Config
{
    public class VideoTouristLikeConfig : IEntityTypeConfiguration<VideoTouristLike>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<VideoTouristLike> builder)
        {
            builder.HasKey(vtl => new { vtl.TouristId, vtl.VideoId });

            builder.HasOne(vtl => vtl.Tourist)
                .WithMany(t => t.VideoLikes)
                .HasForeignKey(vtl => vtl.TouristId);

            builder.HasOne(vtl => vtl.Video)
                .WithMany(v => v.TouristLikes)
                .HasForeignKey(vtl => vtl.VideoId);
        }
    }
    }

