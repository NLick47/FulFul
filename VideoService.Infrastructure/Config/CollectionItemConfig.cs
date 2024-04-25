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
    public class CollectionItemConfig : IEntityTypeConfiguration<CollectionItem>
    {
        public void Configure(EntityTypeBuilder<CollectionItem> builder)
        {
            builder.HasOne(x => x.Video).WithOne();
            builder.HasIndex(x => x.VideoId).IsUnique(false);
        }
    }
}
