using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
 
namespace BookReviewer.Data
{
    public class UserDBContext : IdentityDbContext
    {
        public UserDBContext(DbContextOptions<UserDBContext> options)
           : base(options)
        {
        }

        public DbSet<BookReviewer.Models.User> User { get; set; }
    }
}