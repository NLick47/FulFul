using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VideoService.Domain.Entities;

namespace VideoService.Infrastructure.Config
{
    public class CommentReplyConfig : IEntityTypeConfiguration<CommentReply>
    {
        public void Configure(EntityTypeBuilder<CommentReply> builder)
        {
            builder.HasKey(x => x.Id);

           
        }
    }
}
