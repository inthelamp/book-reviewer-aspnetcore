using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookReviewer.Models;

namespace BookReviewer.Data
{
    public class BookReviewerContext : DbContext
    {
        public BookReviewerContext (DbContextOptions<BookReviewerContext> options)
            : base(options)
        {
        }

        public DbSet<BookReviewer.Models.Author> Author { get; set; }

        public DbSet<BookReviewer.Models.Image> Image { get; set; }

        public DbSet<BookReviewer.Models.Review> Review { get; set; }
    }
}