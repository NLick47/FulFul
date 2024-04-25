using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using VideoService.Domain.Entities;

namespace VideoService.Infrastructure.Config
{
    public class TourisConfigt : IEntityTypeConfiguration<Tourist>
    {
        public void Configure(EntityTypeBuilder<Tourist> builder)
        {
           
        }
    }
}
