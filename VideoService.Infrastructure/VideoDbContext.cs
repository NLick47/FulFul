using Microsoft.EntityFrameworkCore;
using VideoService.Domain.Entities;

namespace VideoService.Infrastructure
{
    public class VideoDbContext : DbContext
    {
        public VideoDbContext(DbContextOptions<VideoDbContext> options)
     : base(options)
        {
        }

        public DbSet<Collection> Collections { get; set; }
        public DbSet<Video> Video { get; set; }

        public DbSet<LikeUser> Likes { get; set; }

        public DbSet<CollectionItem> CollectionItems { get; set; }

        public DbSet<TouristLike> TouristLike { get; set; }

        public DbSet<VideoComment> VideoComment { get; set; }

        public DbSet<CommentReply> CommentReply { get; set; }

        public DbSet<VideoResouce> videoResouces { get; set; }

        public DbSet<VideoLike> VideoLikes { get; set; }

      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
        }
    }
}
