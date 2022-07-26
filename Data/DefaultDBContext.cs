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

        public DbSet<BookReviewer.Models.Reviewer> Reviewer { get; set; }

        public DbSet<BookReviewer.Models.BookCover> BookCover { get; set; }
        public DbSet<BookReviewer.Models.Image> Image { get; set; }

        public DbSet<BookReviewer.Models.Review> Review { get; set; }    
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One to many relationship.
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.ReviewerId); 

            // One to one relationship
            modelBuilder.Entity<Review>()
                .HasOne(r => r.BookCover)
                .WithOne(b => b.Review)
                .HasForeignKey<BookCover>(b => b.ReviewId);                  

            // One to many relationship.
            modelBuilder.Entity<Image>()
                .HasOne(i => i.Review)
                .WithMany(r => r.Images)
                .HasForeignKey(i => i.ReviewId);      
        }
    }
}