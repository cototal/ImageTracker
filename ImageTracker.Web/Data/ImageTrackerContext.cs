using ImageTracker.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace ImageTracker.Web.Data
{
    public class ImageTrackerContext : DbContext
    {
        public ImageTrackerContext(DbContextOptions<ImageTrackerContext> options) : base(options)
        {

        }

        public DbSet<Image> Images { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ImageTag> ImageTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();
            builder.Entity<Image>()
                .HasIndex(c => c.Name)
                .IsUnique();
            builder.Entity<Tag>()
                .HasIndex(c => c.Name)
                .IsUnique();
            builder.Entity<ImageTag>()
                .HasIndex(it => new { it.ImageId, it.TagId })
                .IsUnique();
        }
    }
}
