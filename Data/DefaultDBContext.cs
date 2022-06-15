using Microsoft.EntityFrameworkCore;
using BookReviewer.Models;

namespace BookReviewer.Data
{
    public class DefaultDBContext : DbContext
    {
        public DefaultDBContext (DbContextOptions<DefaultDBContext> options)
            : base(options)
        {
        }

        public DbSet<BookReviewer.Models.Author> Author { get; set; }

        public DbSet<BookReviewer.Models.Image> Image { get; set; }

        public DbSet<BookReviewer.Models.Review> Review { get; set; }    
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One to many relationship.
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Author)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.AuthorId);       

            // One to many relationship.
            modelBuilder.Entity<Image>()
                .HasOne(i => i.Review)
                .WithMany(r => r.Images)
                .HasForeignKey(i => i.ReviewId);
        }
    }
}