using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoService.Domain.Entities;

namespace VideoService.Infrastructure.Config
{
    internal class VideoResouceConfig : IEntityTypeConfiguration<VideoResouce>
    {
        public void Configure(EntityTypeBuilder<VideoResouce> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(e => e.Id).IsUnique(false);

        }
    }
}
